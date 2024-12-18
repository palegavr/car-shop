﻿// <auto-generated />
using System;
using CarShop.CarStorage.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    [DbContext(typeof(CarStorageDatabase))]
    [Migration("20241016153658_AddCarConfigurations")]
    partial class AddCarConfigurations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.AdditionalCarOption", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("CarId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("boolean")
                        .HasAnnotation("Relational:JsonPropertyName", "isRequired");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasAnnotation("Relational:JsonPropertyName", "price");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "type");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.ToTable("AdditionalCarOption");
                });

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.Car", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string[]>("BigImageURLs")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CorpusType")
                        .HasColumnType("integer");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<double>("EngineCapacity")
                        .HasColumnType("double precision");

                    b.Property<int>("FuelType")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("PriceForStandartConfiguration")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.CarConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AirConditioner")
                        .HasColumnType("boolean");

                    b.Property<long>("CarId")
                        .HasColumnType("bigint");

                    b.Property<string>("DifferentCarColor")
                        .HasColumnType("text");

                    b.Property<bool>("HeatedDriversSeat")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeatHeightAdjustment")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.ToTable("CarConfigurations");
                });

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.AdditionalCarOption", b =>
                {
                    b.HasOne("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.Car", null)
                        .WithMany("AdditionalCarOptions")
                        .HasForeignKey("CarId");
                });

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.CarConfiguration", b =>
                {
                    b.HasOne("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.Car", null)
                        .WithMany("CarConfigurations")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarShop.ServiceDefaults.ServiceInterfaces.CarStorage.Car", b =>
                {
                    b.Navigation("AdditionalCarOptions");

                    b.Navigation("CarConfigurations");
                });
#pragma warning restore 612, 618
        }
    }
}
