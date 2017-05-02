using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Wechat.Entities;

namespace Wechat.Migrations
{
    [DbContext(typeof(WechatContext))]
    [Migration("20170430120457_update")]
    partial class update
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Wechat.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Display");

                    b.Property<string>("OpenId");

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Wechat.Entities.CleanConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CheckLoginRetryCount");

                    b.Property<string>("MessagePrefix");

                    b.Property<string>("MessageSuffix");

                    b.Property<int>("SendMsgFailRetryCount");

                    b.Property<int>("SendMsgFailRetryInteval");

                    b.Property<int>("SendMsgInteval");

                    b.HasKey("Id");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("Wechat.Entities.CleanTask", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<string>("ContactDetail");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime");

                    b.Property<int?>("CreatorId");

                    b.Property<int>("FriendCount");

                    b.Property<int>("GroupCount");

                    b.Property<int>("PublicCount");

                    b.Property<int>("Status");

                    b.Property<string>("UserDetail");

                    b.Property<string>("UserDisplay");

                    b.Property<int>("ZombieCount");

                    b.Property<string>("ZombieList");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("Wechat.Entities.CleanTask", b =>
                {
                    b.HasOne("Wechat.Entities.Account", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");
                });
        }
    }
}
