using BankDL.DataAccess;
using BankDL.Interfaces;
using BankDL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SimpleBank
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//Dependencies registration
			builder.Services.AddDbContext<BankDbContext>(options =>
			{
				options.UseInMemoryDatabase("SimpleBankDb");
			});

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
