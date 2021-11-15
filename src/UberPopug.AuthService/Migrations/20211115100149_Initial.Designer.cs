﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UberPopug.AuthService;

namespace UberPopug.AuthService.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    [Migration("20211115100149_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UberPopug.AuthService.Users.User", b =>
                {
                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Email");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Email = "admin@popug.ru",
                            Password = "123",
                            Role = 3
                        },
                        new
                        {
                            Email = "manager@popug.ru",
                            Password = "123",
                            Role = 1
                        },
                        new
                        {
                            Email = "accountant@popug.ru",
                            Password = "123",
                            Role = 2
                        },
                        new
                        {
                            Email = "emp1@popug.ru",
                            Password = "123",
                            Role = 0
                        },
                        new
                        {
                            Email = "emp2@popug.ru",
                            Password = "123",
                            Role = 0
                        },
                        new
                        {
                            Email = "emp3@popug.ru",
                            Password = "123",
                            Role = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
