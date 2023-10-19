﻿// <auto-generated />
using System;
using API_BUSESCONTROL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_BUSESCONTROL.Migrations
{
    [DbContext(typeof(BancoContext))]
    partial class BancoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("API_BUSESCONTROL.Models.Cliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Adimplente")
                        .HasColumnType("int");

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ComplementoResidencial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ddd")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NumeroResidencial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("varchar(9)");

                    b.HasKey("Id");

                    b.ToTable("Cliente");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Cliente");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.ClientesContrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ContratoId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataEmissaoPdfRescisao")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("PessoaFisicaId")
                        .HasColumnType("int");

                    b.Property<int?>("PessoaJuridicaId")
                        .HasColumnType("int");

                    b.Property<int>("ProcessRescisao")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("PessoaFisicaId");

                    b.HasIndex("PessoaJuridicaId");

                    b.ToTable("ClientesContrato");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Contrato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Andamento")
                        .HasColumnType("int");

                    b.Property<int>("Aprovacao")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataEmissao")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DataVencimento")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Detalhamento")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("MotoristaId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("OnibusId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("Pagament")
                        .HasColumnType("int");

                    b.Property<int?>("QtParcelas")
                        .HasColumnType("int");

                    b.Property<int>("StatusContrato")
                        .HasColumnType("int");

                    b.Property<decimal?>("ValorMonetario")
                        .IsRequired()
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorParcelaContrato")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorParcelaContratoPorCliente")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorTotalPagoContrato")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("MotoristaId");

                    b.HasIndex("OnibusId");

                    b.ToTable("Contrato");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Financeiro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ContratoId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataEmissao")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DataVencimento")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DespesaReceita")
                        .HasColumnType("int");

                    b.Property<string>("Detalhamento")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("FinanceiroStatus")
                        .HasColumnType("int");

                    b.Property<int?>("FornecedorId")
                        .HasColumnType("int");

                    b.Property<int>("Pagament")
                        .HasColumnType("int");

                    b.Property<int?>("PessoaFisicaId")
                        .HasColumnType("int");

                    b.Property<int?>("PessoaJuridicaId")
                        .HasColumnType("int");

                    b.Property<int?>("QtParcelas")
                        .HasColumnType("int");

                    b.Property<int>("TypeEfetuacao")
                        .HasColumnType("int");

                    b.Property<decimal?>("ValorParcelaDR")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorTotDR")
                        .IsRequired()
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorTotTaxaJurosPaga")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("ValorTotalPago")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("FornecedorId");

                    b.HasIndex("PessoaFisicaId");

                    b.HasIndex("PessoaJuridicaId");

                    b.ToTable("Financeiro");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Fornecedor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Cnpj")
                        .HasColumnType("longtext");

                    b.Property<string>("Cpf")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("DataFornecedor")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Ddd")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NameOrRazaoSocial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NumeroResidencial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("varchar(9)");

                    b.Property<int>("TypePessoa")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Fornecedor");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Funcionario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Apelido")
                        .HasColumnType("longtext");

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Cargo")
                        .HasColumnType("int");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)");

                    b.Property<string>("ChaveRedefinition")
                        .HasColumnType("longtext");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ComplementoResidencial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("DataNascimento")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Ddd")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NumeroResidencial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Senha")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("StatusUsuario")
                        .HasColumnType("int");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("varchar(9)");

                    b.HasKey("Id");

                    b.ToTable("Funcionario");
                });

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

            modelBuilder.Entity("API_BUSESCONTROL.Models.Parcela", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataEfetuacao")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DataVencimentoParcela")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("FinanceiroId")
                        .HasColumnType("int");

                    b.Property<string>("NomeParcela")
                        .HasColumnType("longtext");

                    b.Property<int?>("StatusPagamento")
                        .HasColumnType("int");

                    b.Property<decimal?>("ValorJuros")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("FinanceiroId");

                    b.ToTable("Parcela");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Rescisao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ContratoId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataRescisao")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal?>("Multa")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int?>("PessoaFisicaId")
                        .HasColumnType("int");

                    b.Property<int?>("PessoaJuridicaId")
                        .HasColumnType("int");

                    b.Property<decimal?>("ValorPagoContrato")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("PessoaFisicaId");

                    b.HasIndex("PessoaJuridicaId");

                    b.ToTable("Rescisao");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.SubContratoMotorista", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ContratoId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataFinal")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DataInicial")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("FuncionarioId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("FuncionarioId");

                    b.ToTable("SubContratoMotorista");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.SubContratoOnibus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ContratoId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataFinal")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DataInicial")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("OnibusId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContratoId");

                    b.HasIndex("OnibusId");

                    b.ToTable("SubContratoOnibus");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.PessoaFisica", b =>
                {
                    b.HasBaseType("API_BUSESCONTROL.Models.Cliente");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("DataNascimento")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("IdVinculacaoContratual")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NameMae")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Rg")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("PessoaFisica");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.PessoaJuridica", b =>
                {
                    b.HasBaseType("API_BUSESCONTROL.Models.Cliente");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("InscricaoEstadual")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("InscricaoMunicipal")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NomeFantasia")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RazaoSocial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("PessoaJuridica_Status");

                    b.HasDiscriminator().HasValue("PessoaJuridica");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.ClientesContrato", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Contrato", "Contrato")
                        .WithMany("ClientesContrato")
                        .HasForeignKey("ContratoId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaFisica", "PessoaFisica")
                        .WithMany("ClientesContrato")
                        .HasForeignKey("PessoaFisicaId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaJuridica", "PessoaJuridica")
                        .WithMany("ClientesContrato")
                        .HasForeignKey("PessoaJuridicaId");

                    b.Navigation("Contrato");

                    b.Navigation("PessoaFisica");

                    b.Navigation("PessoaJuridica");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Contrato", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Funcionario", "Motorista")
                        .WithMany("Contratos")
                        .HasForeignKey("MotoristaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_BUSESCONTROL.Models.Onibus", "Onibus")
                        .WithMany("Contratos")
                        .HasForeignKey("OnibusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Motorista");

                    b.Navigation("Onibus");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Financeiro", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Contrato", "Contrato")
                        .WithMany("Financeiros")
                        .HasForeignKey("ContratoId");

                    b.HasOne("API_BUSESCONTROL.Models.Fornecedor", "Fornecedor")
                        .WithMany("Financeiros")
                        .HasForeignKey("FornecedorId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaFisica", "PessoaFisica")
                        .WithMany("Financeiros")
                        .HasForeignKey("PessoaFisicaId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaJuridica", "PessoaJuridica")
                        .WithMany("Financeiros")
                        .HasForeignKey("PessoaJuridicaId");

                    b.Navigation("Contrato");

                    b.Navigation("Fornecedor");

                    b.Navigation("PessoaFisica");

                    b.Navigation("PessoaJuridica");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Parcela", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Financeiro", "Financeiro")
                        .WithMany("Parcelas")
                        .HasForeignKey("FinanceiroId");

                    b.Navigation("Financeiro");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Rescisao", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Contrato", "Contrato")
                        .WithMany("Rescisoes")
                        .HasForeignKey("ContratoId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaFisica", "PessoaFisica")
                        .WithMany("Rescisoes")
                        .HasForeignKey("PessoaFisicaId");

                    b.HasOne("API_BUSESCONTROL.Models.PessoaJuridica", "PessoaJuridica")
                        .WithMany("Rescisoes")
                        .HasForeignKey("PessoaJuridicaId");

                    b.Navigation("Contrato");

                    b.Navigation("PessoaFisica");

                    b.Navigation("PessoaJuridica");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.SubContratoMotorista", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Contrato", "Contrato")
                        .WithMany("SubContratoMotoristas")
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_BUSESCONTROL.Models.Funcionario", "Funcionario")
                        .WithMany("SubContratoMotoristas")
                        .HasForeignKey("FuncionarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contrato");

                    b.Navigation("Funcionario");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.SubContratoOnibus", b =>
                {
                    b.HasOne("API_BUSESCONTROL.Models.Contrato", "Contrato")
                        .WithMany()
                        .HasForeignKey("ContratoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_BUSESCONTROL.Models.Onibus", "Onibus")
                        .WithMany()
                        .HasForeignKey("OnibusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contrato");

                    b.Navigation("Onibus");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Contrato", b =>
                {
                    b.Navigation("ClientesContrato");

                    b.Navigation("Financeiros");

                    b.Navigation("Rescisoes");

                    b.Navigation("SubContratoMotoristas");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Financeiro", b =>
                {
                    b.Navigation("Parcelas");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Fornecedor", b =>
                {
                    b.Navigation("Financeiros");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Funcionario", b =>
                {
                    b.Navigation("Contratos");

                    b.Navigation("SubContratoMotoristas");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.Onibus", b =>
                {
                    b.Navigation("Contratos");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.PessoaFisica", b =>
                {
                    b.Navigation("ClientesContrato");

                    b.Navigation("Financeiros");

                    b.Navigation("Rescisoes");
                });

            modelBuilder.Entity("API_BUSESCONTROL.Models.PessoaJuridica", b =>
                {
                    b.Navigation("ClientesContrato");

                    b.Navigation("Financeiros");

                    b.Navigation("Rescisoes");
                });
#pragma warning restore 612, 618
        }
    }
}
