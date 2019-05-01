﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using server.Models;

namespace server.Migrations
{
    [DbContext(typeof(DoctorsContext))]
    [Migration("20190430110432_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("server.Models.DateTimePair", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("DoctorId");

                    b.Property<string>("date");

                    b.Property<string>("time");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("DateTimePair");
                });

            modelBuilder.Entity("server.Models.Doctor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("name");

                    b.HasKey("Id");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("server.Models.Procedure", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("DoctorId");

                    b.Property<string>("name");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Procedure");
                });

            modelBuilder.Entity("server.Models.DateTimePair", b =>
                {
                    b.HasOne("server.Models.Doctor")
                        .WithMany("dateTimes")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("server.Models.Procedure", b =>
                {
                    b.HasOne("server.Models.Doctor")
                        .WithMany("procedures")
                        .HasForeignKey("DoctorId");
                });
#pragma warning restore 612, 618
        }
    }
}
