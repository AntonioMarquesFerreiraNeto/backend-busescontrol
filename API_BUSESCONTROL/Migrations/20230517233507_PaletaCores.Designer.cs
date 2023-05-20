﻿// <auto-generated />
using System;
using API_BUSESCONTROL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    [DbContext(typeof(BancoContext))]
    [Migration("20230517233507_PaletaCores")]
    partial class PaletaCores
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("API_BUSESCONTROL.Models.Onibus", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Assentos")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Chassi")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CorBus")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DataFabricacao")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Marca")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NameBus")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Placa")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Renavam")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("StatusOnibus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Onibus");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.PaletaCores", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Cor")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("PaletaCores");
                });
#pragma warning restore 612, 618
        }
    }
}
