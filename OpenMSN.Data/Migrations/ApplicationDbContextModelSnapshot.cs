﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OpenMSN.Data;

#nullable disable

namespace OpenMSN.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OpenMSN.Data.Entities.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("contact_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ContactId"));

                    b.Property<string>("List")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("list");

                    b.Property<int>("TargetUserId")
                        .HasColumnType("integer")
                        .HasColumnName("target_user_id");

                    b.Property<DateTimeOffset>("TimeAdded")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_added");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("ContactId")
                        .HasName("pk_contacts");

                    b.HasIndex("TargetUserId")
                        .HasDatabaseName("ix_contacts_target_user_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_contacts_user_id");

                    b.ToTable("contacts", (string)null);
                });

            modelBuilder.Entity("OpenMSN.Data.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<bool>("Activated")
                        .HasColumnType("boolean")
                        .HasColumnName("activated");

                    b.Property<string>("ActivationToken")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("activation_token");

                    b.Property<bool>("ContactAddAlert")
                        .HasColumnType("boolean")
                        .HasColumnName("contact_add_alert");

                    b.Property<int>("ContactListVersion")
                        .HasColumnType("integer")
                        .HasColumnName("contact_list_version");

                    b.Property<string>("ContactPrivacy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("contact_privacy");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email_address");

                    b.Property<string>("MD5Salt")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("md5salt");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PasswordHashMD5")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash_md5");

                    b.Property<DateTimeOffset>("TimeAdded")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_added");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email_address");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Activated = true,
                            ActivationToken = "78196fd1-8181-45d0-97c8-16a59977acb4",
                            ContactAddAlert = true,
                            ContactListVersion = 0,
                            ContactPrivacy = "AL",
                            EmailAddress = "pizzaboxer@hotmail.com",
                            MD5Salt = "7f30143936ba04972090",
                            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$/+34CRzCS6MJv30NTB1fDQ$IoisuDjna+EYXU617erIYmXMAU6/NJdQeXx7XA+w8L0",
                            PasswordHashMD5 = "$argon2id$v=19$m=65536,t=3,p=1$GHWJXxxgaAz1gNFOZTeHkg$BSni28HhYC9Dx1LcVTnmyJBl4GEPR0YcfPxWTmO0hyc",
                            TimeAdded = new DateTimeOffset(new DateTime(2022, 10, 1, 20, 21, 23, 6, DateTimeKind.Unspecified).AddTicks(6544), new TimeSpan(0, 0, 0, 0, 0)),
                            Username = "pizzaboxer@hotmail.com"
                        },
                        new
                        {
                            UserId = 2,
                            Activated = true,
                            ActivationToken = "63e9f88f-8f89-4470-a329-6f29d8b15a49",
                            ContactAddAlert = true,
                            ContactListVersion = 0,
                            ContactPrivacy = "AL",
                            EmailAddress = "pizzaboxer2@hotmail.com",
                            MD5Salt = "6502408dbcbea1e56eeb",
                            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$rfVcCqUoJNQEXNFcoD/KtA$tC0Sik/4zzCN3WSMnG6L6fHFAc2I+QeRGmjREZMsOkM",
                            PasswordHashMD5 = "$argon2id$v=19$m=65536,t=3,p=1$F/NwmzVUbf2BkmONdub81g$NW9o0CvFt/H3forXfcHKSWxNRdgirel9px5S6l1IpzQ",
                            TimeAdded = new DateTimeOffset(new DateTime(2022, 10, 1, 20, 21, 23, 6, DateTimeKind.Unspecified).AddTicks(6552), new TimeSpan(0, 0, 0, 0, 0)),
                            Username = "pizzaboxer2@hotmail.com"
                        });
                });

            modelBuilder.Entity("OpenMSN.Data.Entities.Contact", b =>
                {
                    b.HasOne("OpenMSN.Data.Entities.User", "TargetUser")
                        .WithOne()
                        .HasForeignKey("OpenMSN.Data.Entities.Contact", "TargetUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_contacts_users_target_user_id");

                    b.HasOne("OpenMSN.Data.Entities.User", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_contacts_users_user_id");

                    b.Navigation("TargetUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OpenMSN.Data.Entities.User", b =>
                {
                    b.Navigation("Contacts");
                });
#pragma warning restore 612, 618
        }
    }
}
