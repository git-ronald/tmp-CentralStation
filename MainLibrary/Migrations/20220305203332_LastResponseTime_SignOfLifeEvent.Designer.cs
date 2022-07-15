﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CentralStation.Data.Migrations
{
    [DbContext(typeof(MainDbContext))]
    [Migration("20220305203332_LastResponseTime_SignOfLifeEvent")]
    partial class LastResponseTime_SignOfLifeEvent
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.2");

            modelBuilder.Entity("CentralStation.Data.Models.Peer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Peers");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerConnection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IP")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastResponseTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PeerNodeId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionId");

                    b.HasIndex("LastResponseTime");

                    b.HasIndex("PeerNodeId");

                    b.ToTable("PeerConnections");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PeerNodeId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RequestTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ResponseTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PeerNodeId");

                    b.HasIndex("RequestTime");

                    b.HasIndex("ResponseTime");

                    b.ToTable("PeerMessages");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerNode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastIP")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PeerId")
                        .HasColumnType("TEXT");

                    b.Property<short>("SignOfLifeEvent")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.HasIndex("PeerId");

                    b.ToTable("PeerNodes");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerConnection", b =>
                {
                    b.HasOne("CentralStation.Data.Models.PeerNode", "PeerNode")
                        .WithMany("PeerConnections")
                        .HasForeignKey("PeerNodeId");

                    b.Navigation("PeerNode");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerMessage", b =>
                {
                    b.HasOne("CentralStation.Data.Models.PeerNode", "PeerNode")
                        .WithMany("PeerMessages")
                        .HasForeignKey("PeerNodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PeerNode");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerNode", b =>
                {
                    b.HasOne("CentralStation.Data.Models.Peer", "Peer")
                        .WithMany("PeerNodes")
                        .HasForeignKey("PeerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Peer");
                });

            modelBuilder.Entity("CentralStation.Data.Models.Peer", b =>
                {
                    b.Navigation("PeerNodes");
                });

            modelBuilder.Entity("CentralStation.Data.Models.PeerNode", b =>
                {
                    b.Navigation("PeerConnections");

                    b.Navigation("PeerMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
