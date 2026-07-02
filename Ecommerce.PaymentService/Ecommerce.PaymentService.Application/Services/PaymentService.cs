using Ecommerce.PaymentService.Application.Dto;
using Ecommerce.PaymentService.Application.Events;
using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Interface.IRepository;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Ecommerce.PaymentService.Application.Models;
using Ecommerce.PaymentService.Domain.Model;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.PaymentService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly AutoMapper.IMapper _mapper;
        private IPaymentGatewayFactory _paymentGatewayFactory;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IConfiguration _configuration;
        
        public PaymentService(IPaymentRepository repository, AutoMapper.IMapper mapper,
            IPaymentGatewayFactory paymentGatewayFactory,IKafkaProducer kafkaProducer,IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _paymentGatewayFactory = paymentGatewayFactory;
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        public async Task CreatePaymentAsync(PaymentDto paymentDto)
        {
            PaymentResult paymentResult;
            var payment = _mapper.Map<Payment>(paymentDto);
            payment.PaymentStatus = "processed";
            int paymentId= await _repository.AddAsync(payment);
            paymentDto.PaymentId = paymentId;
            if (paymentId != null) 
            {
                paymentResult =  await  Pay(paymentDto);
                paymentDto.TransactionId = paymentResult?.TransactionId;
                paymentDto.PaymentStatus = paymentResult.IsSuccess ? "Succeeded" : "Failed";
                paymentDto.UpdatedDate = DateTime.UtcNow;
                paymentDto.PaymentGatewayError = paymentResult.ErrorMessage;
            }
            await UpdatePaymentAsync(paymentDto);
             await PublishPaymentDetails(paymentDto);
        }
        public async Task DeletePaymentAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing != null)
            {
                await _repository.Remove(existing);
            }
        }

        public async Task<IList<PaymentDto>> GetAllPaymentsAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(x => _mapper.Map<PaymentDto>(x)).ToList();
        }

        public async Task<PaymentDto?> GetPaymentAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }
        public async Task UpdatePaymentAsync(PaymentDto paymentDto)
        {
            var payment = _mapper.Map<Payment>(paymentDto);
            await _repository.Update(payment);
        }

        private Task<PaymentResult> Pay(PaymentDto paymentDto) 
        {

            var paymentGatway = _paymentGatewayFactory.GetGateway(paymentDto.PaymentMethod.ToString());
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                Amount = paymentDto.Amount,
                Currency = "INR",
                PaymentId = paymentDto.PaymentId,
            };
              return   paymentGatway.PayAsync(paymentRequest);
        }


        private async Task PublishPaymentDetails(PaymentDto paymentDto) 
        {
            string topicName = _configuration["Kafka:PamentTopic"];
            PaymentCreatedEvent paymentCreatedEvent = new PaymentCreatedEvent()
            {
                MessageId= Guid.NewGuid().ToString(),
                PaymentId=paymentDto.PaymentId,
                OrderId=paymentDto.OrderId,
                Status=paymentDto.PaymentStatus
            };

            await  _kafkaProducer.PublishAsync<PaymentCreatedEvent>(topicName,paymentCreatedEvent); 

        }
    }
}