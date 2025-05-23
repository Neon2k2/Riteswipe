﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RiteSwipe.Infrastructure.Persistence;

#nullable disable

namespace RiteSwipe.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RiteSwipe.Domain.Entities.EscrowPayment", b =>
                {
                    b.Property<int>("EscrowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EscrowId"));

                    b.Property<decimal>("AmountHeld")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReleasedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("EscrowId");

                    b.HasIndex("TaskId")
                        .IsUnique();

                    b.ToTable("EscrowPayments");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Skill", b =>
                {
                    b.Property<int>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SkillId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("SkillName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("SkillId");

                    b.HasIndex("SkillName")
                        .IsUnique();

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Swipe", b =>
                {
                    b.Property<int>("SwipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SwipeId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("SwipeDirection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SwipeId");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId", "TaskId")
                        .IsUnique();

                    b.ToTable("Swipes");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskApplication", b =>
                {
                    b.Property<int>("ApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicationId"));

                    b.Property<string>("ApplicationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<int>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("ApplicationId");

                    b.HasIndex("WorkerId");

                    b.HasIndex("TaskId", "WorkerId")
                        .IsUnique();

                    b.ToTable("TaskApplications");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskDispute", b =>
                {
                    b.Property<int>("DisputeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DisputeId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("RaisedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("DisputeId");

                    b.HasIndex("RaisedByUserId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskDisputes");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskItem", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("CurrentRate")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<int>("DemandIndex")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MaxPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MinPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("PostedByUserId")
                        .HasColumnType("int");

                    b.Property<int?>("SkillRequiredId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskId");

                    b.HasIndex("PostedByUserId");

                    b.HasIndex("SkillRequiredId");

                    b.HasIndex("Status");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskReview", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReviewId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("ReviewText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReviewedUserId")
                        .HasColumnType("int");

                    b.Property<int>("ReviewerUserId")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("ReviewId");

                    b.HasIndex("ReviewedUserId");

                    b.HasIndex("ReviewerUserId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskReviews");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.UserSkill", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "SkillId");

                    b.HasIndex("SkillId");

                    b.ToTable("UserSkills");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.EscrowPayment", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.TaskItem", "Task")
                        .WithOne("EscrowPayment")
                        .HasForeignKey("RiteSwipe.Domain.Entities.EscrowPayment", "TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Notification", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Swipe", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.TaskItem", "Task")
                        .WithMany("Swipes")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.User", "User")
                        .WithMany("Swipes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskApplication", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.TaskItem", "Task")
                        .WithMany("Applications")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.User", "Worker")
                        .WithMany("TaskApplications")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskDispute", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.User", "RaisedByUser")
                        .WithMany("DisputesRaised")
                        .HasForeignKey("RaisedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.TaskItem", "Task")
                        .WithMany("Disputes")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RaisedByUser");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskItem", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.User", "PostedByUser")
                        .WithMany("PostedTasks")
                        .HasForeignKey("PostedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.Skill", "RequiredSkill")
                        .WithMany("Tasks")
                        .HasForeignKey("SkillRequiredId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("PostedByUser");

                    b.Navigation("RequiredSkill");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskReview", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.User", "ReviewedUser")
                        .WithMany("ReviewsReceived")
                        .HasForeignKey("ReviewedUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.User", "ReviewerUser")
                        .WithMany("ReviewsGiven")
                        .HasForeignKey("ReviewerUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.TaskItem", "Task")
                        .WithMany("Reviews")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReviewedUser");

                    b.Navigation("ReviewerUser");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.UserSkill", b =>
                {
                    b.HasOne("RiteSwipe.Domain.Entities.Skill", "Skill")
                        .WithMany("UserSkills")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RiteSwipe.Domain.Entities.User", "User")
                        .WithMany("UserSkills")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Skill");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.Skill", b =>
                {
                    b.Navigation("Tasks");

                    b.Navigation("UserSkills");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.TaskItem", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("Disputes");

                    b.Navigation("EscrowPayment");

                    b.Navigation("Reviews");

                    b.Navigation("Swipes");
                });

            modelBuilder.Entity("RiteSwipe.Domain.Entities.User", b =>
                {
                    b.Navigation("DisputesRaised");

                    b.Navigation("Notifications");

                    b.Navigation("PostedTasks");

                    b.Navigation("ReviewsGiven");

                    b.Navigation("ReviewsReceived");

                    b.Navigation("Swipes");

                    b.Navigation("TaskApplications");

                    b.Navigation("UserSkills");
                });
#pragma warning restore 612, 618
        }
    }
}
