﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Natia.Core.Context;

#nullable disable

namespace Natia.Core.Migrations
{
    [DbContext(typeof(SpeakerDbContext))]
    [Migration("20250131230312_mgrt")]
    partial class mgrt
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Natia.Core.Entities.Chanells", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ChanellFormat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ChanellFormat");

                    b.Property<bool>("FromOptic")
                        .HasColumnType("bit")
                        .HasColumnName("Is_From_Optic");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name_Of_Chanell");

                    b.Property<string>("NameForSPeak")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PortIn250")
                        .HasColumnType("int")
                        .HasColumnName("Port_In_250");

                    b.HasKey("Id");

                    b.ToTable("Chanells");
                });

            modelBuilder.Entity("Natia.Core.Entities.Desclamblers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Card")
                        .HasColumnType("int")
                        .HasColumnName("Card_In_Desclambler");

                    b.Property<int>("ChanellId")
                        .HasColumnType("int")
                        .HasColumnName("Chanell_Id");

                    b.Property<int>("EmrNumber")
                        .HasColumnType("int")
                        .HasColumnName("Emr_Number");

                    b.Property<int>("Port")
                        .HasColumnType("int")
                        .HasColumnName("Port_In_Desclambler");

                    b.HasKey("Id");

                    b.HasIndex("ChanellId");

                    b.ToTable("Desclamblers");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr100Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr100info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr110info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr110info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr120Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr120info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr130Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr130info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr200Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr200info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Emr60Info", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceEmr")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("emr60info");
                });

            modelBuilder.Entity("Natia.Core.Entities.Greetings", b =>
                {
                    b.Property<int>("GreetingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GreetingId"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GreetingId");

                    b.ToTable("Greetings");
                });

            modelBuilder.Entity("Natia.Core.Entities.Infos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AlarmMessage")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Alarm_For_Display");

                    b.Property<int>("CHanellId")
                        .HasColumnType("int")
                        .HasColumnName("CHanell_Id");

                    b.HasKey("Id");

                    b.HasIndex("CHanellId");

                    b.ToTable("Infos");
                });

            modelBuilder.Entity("Natia.Core.Entities.Neurall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ChannelName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCritical")
                        .HasColumnType("bit");

                    b.Property<bool>("IsError")
                        .HasColumnType("bit");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Satellite")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SuggestedSolution")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhatNatiaSaid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WhatWasTopic")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("neurals");
                });

            modelBuilder.Entity("Natia.Core.Entities.Reciever", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Card")
                        .HasColumnType("int")
                        .HasColumnName("Card_In_Reciever");

                    b.Property<int>("ChanellId")
                        .HasColumnType("int")
                        .HasColumnName("Chanell_Id");

                    b.Property<int>("EmrNumber")
                        .HasColumnType("int")
                        .HasColumnName("Emr_Number");

                    b.Property<int>("Port")
                        .HasColumnType("int")
                        .HasColumnName("Port_In_Reciever");

                    b.HasKey("Id");

                    b.HasIndex("ChanellId");

                    b.ToTable("recievers");
                });

            modelBuilder.Entity("Natia.Core.Entities.Transcoder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Card")
                        .HasColumnType("int")
                        .HasColumnName("Card_In_Transcoder");

                    b.Property<int>("ChanellId")
                        .HasColumnType("int")
                        .HasColumnName("Chanell_Id");

                    b.Property<int>("EmrNumber")
                        .HasColumnType("int")
                        .HasColumnName("Emr_Number");

                    b.Property<int>("Port")
                        .HasColumnType("int")
                        .HasColumnName("Port_In_Transcoder");

                    b.HasKey("Id");

                    b.HasIndex("ChanellId");

                    b.ToTable("Transcoders");
                });

            modelBuilder.Entity("Natia.Core.Entities.Desclamblers", b =>
                {
                    b.HasOne("Natia.Core.Entities.Chanells", "Chanell")
                        .WithMany("Desclambler")
                        .HasForeignKey("ChanellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chanell");
                });

            modelBuilder.Entity("Natia.Core.Entities.Infos", b =>
                {
                    b.HasOne("Natia.Core.Entities.Chanells", "chanell")
                        .WithMany("Infos")
                        .HasForeignKey("CHanellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("chanell");
                });

            modelBuilder.Entity("Natia.Core.Entities.Reciever", b =>
                {
                    b.HasOne("Natia.Core.Entities.Chanells", "Chanell")
                        .WithMany("Recievers")
                        .HasForeignKey("ChanellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chanell");
                });

            modelBuilder.Entity("Natia.Core.Entities.Transcoder", b =>
                {
                    b.HasOne("Natia.Core.Entities.Chanells", "Chanell")
                        .WithMany("Transcoder")
                        .HasForeignKey("ChanellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chanell");
                });

            modelBuilder.Entity("Natia.Core.Entities.Chanells", b =>
                {
                    b.Navigation("Desclambler");

                    b.Navigation("Infos");

                    b.Navigation("Recievers");

                    b.Navigation("Transcoder");
                });
#pragma warning restore 612, 618
        }
    }
}
