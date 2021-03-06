// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CentralStation.Data.Migrations
{
    [DbContext(typeof(MainDbContext))]
    [Migration("20220327121423_MajorRefactorPeerRelated")]
    partial class MajorRefactorPeerRelated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.2");

            modelBuilder.Entity("CentralStation.Data.Models.NavPage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInNavMenu")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NavLinkMatch")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PeerNodeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IsInNavMenu");

                    b.HasIndex("Order");

                    b.HasIndex("ParentId");

                    b.HasIndex("PeerNodeId");

                    b.ToTable("NavPages");
                });

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

                    b.Property<DateTime>("LastMessageTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PeerNodeId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionId");

                    b.HasIndex("LastMessageTime");

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

                    b.Property<string>("RequestData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RequestMethod")
                        .IsRequired()
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

                    b.Property<int>("SignOfLifeEvent")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.HasIndex("Name");

                    b.HasIndex("PeerId");

                    b.ToTable("PeerNodes");
                });

            modelBuilder.Entity("CentralStation.Data.Models.NavPage", b =>
                {
                    b.HasOne("CentralStation.Data.Models.NavPage", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("CentralStation.Data.Models.PeerNode", "PeerNode")
                        .WithMany()
                        .HasForeignKey("PeerNodeId");

                    b.Navigation("Parent");

                    b.Navigation("PeerNode");
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

            modelBuilder.Entity("CentralStation.Data.Models.NavPage", b =>
                {
                    b.Navigation("Children");
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
