using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UberPopug.Common.Constants;
using UberPopug.Common.Interfaces;

namespace UberPopug.TaskTrackerService.Users
{
	public class RegisterUserConsumer : BackgroundService
	{
		private readonly IKafkaConsumer<string, CreateUserCommand> _consumer;
		public RegisterUserConsumer(IKafkaConsumer<string, CreateUserCommand> kafkaConsumer)
		{
			_consumer = kafkaConsumer;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				await _consumer.StartConsumingAsync(KafkaTopics.CreateUser, stoppingToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{(int)HttpStatusCode.InternalServerError} ConsumeFailedOnTopic - {KafkaTopics.CreateUser}, {ex}");
			}
		}

		public override void Dispose()
		{
			_consumer.Close();
			_consumer.Dispose();

			base.Dispose();
		}
	}
}
