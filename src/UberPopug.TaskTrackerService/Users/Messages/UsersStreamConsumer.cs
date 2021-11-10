using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UberPopug.Common.Constants;
using UberPopug.Common.Interfaces;

namespace UberPopug.TaskTrackerService.Users.Messages
{
	public class UsersStreamConsumer : BackgroundService
	{
		private readonly IKafkaConsumer<string, UserCreatedEvent> _consumer;
		
		public UsersStreamConsumer(IKafkaConsumer<string, UserCreatedEvent> kafkaConsumer)
		{
			_consumer = kafkaConsumer;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				await _consumer.StartConsumingAsync(KafkaTopics.UsersStream, stoppingToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{(int)HttpStatusCode.InternalServerError} ConsumeFailedOnTopic - {KafkaTopics.UsersStream}, {ex}");
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
