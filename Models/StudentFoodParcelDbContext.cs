using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentFoodParcelApp.Models;

public partial class StudentFoodParcelDbContext : DbContext
{
    public StudentFoodParcelDbContext()
    {
    }

    public StudentFoodParcelDbContext(DbContextOptions<StudentFoodParcelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Distributionrequest> Distributionrequests { get; set; } = null!;
    public virtual DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
    public virtual DbSet<Campus> Campuses { get; set; }
    public virtual DbSet<Cycleitem> Cycleitems { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Distribution> Distributions { get; set; }
    public virtual DbSet<Financestaff> Financestaffs { get; set; }
    public virtual DbSet<Foodparcelcycle> Foodparcelcycles { get; set; }
    public virtual DbSet<Fundingstatus> Fundingstatuses { get; set; }
    public virtual DbSet<Itadmin> Itadmins { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Request> Requests { get; set; }
    public virtual DbSet<Srcadmin> Srcadmins { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<University> Universities { get; set; }
    public virtual DbSet<Venue> Venues { get; set; }
    public virtual DbSet<VwApplicationOverview> VwApplicationOverviews { get; set; }
    public virtual DbSet<VwStudentProfile> VwStudentProfiles { get; set; }
    public virtual DbSet<VwStudentRequest> VwStudentRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationStatus>(entity =>
        {
            entity.HasKey(e => e.Applicationstatusid).HasName("PK__APPLICAT__5539B7F582794F89");
            entity.ToTable("APPLICATION_STATUSES");
            entity.Property(e => e.Applicationstatusid).HasColumnName("APPLICATIONSTATUSID");
            entity.Property(e => e.Applicationstatus).HasMaxLength(255).HasColumnName("APPLICATIONSTATUS");

            // REMOVED HasMaxLength
            entity.Property(e => e.DecisionDate).HasColumnName("DECISION_DATE");

            entity.Property(e => e.DecisionReason).HasMaxLength(255).HasColumnName("DECISION_REASON");
            entity.Property(e => e.Requestid).HasColumnName("REQUESTID");
            entity.Property(e => e.Srcadminid).HasColumnName("SRCADMINID");

            entity.HasOne(d => d.Request).WithMany(p => p.ApplicationStatuses)
                .HasForeignKey(d => d.Requestid)
                .HasConstraintName("FK__APPLICATI__REQUE__619B8048");

            entity.HasOne(d => d.Srcadmin).WithMany(p => p.ApplicationStatuses)
                .HasForeignKey(d => d.Srcadminid)
                .HasConstraintName("FK__APPLICATI__SRCAD__628FA481");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Campusid).HasName("PK__CAMPUSES__A6EA7D1861E5CC45");
            entity.ToTable("CAMPUSES");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Campusname).HasMaxLength(100).HasColumnName("CAMPUSNAME");
            entity.Property(e => e.Universityid).HasColumnName("UNIVERSITYID");

            entity.HasOne(d => d.University).WithMany(p => p.Campuses)
                .HasForeignKey(d => d.Universityid)
                .HasConstraintName("FK__CAMPUSES__UNIVER__398D8EEE");
        });

        modelBuilder.Entity<Cycleitem>(entity =>
        {
            entity.HasKey(e => e.Cycleitemid).HasName("PK__CYCLEITE__D56779211F51E92E");
            entity.ToTable("CYCLEITEMS");
            entity.HasIndex(e => new { e.Cycleid, e.Itemname }, "CYCLEITEMS_CON_UNQ").IsUnique();
            entity.Property(e => e.Cycleitemid).HasColumnName("CYCLEITEMID");
            entity.Property(e => e.Cycleid).HasColumnName("CYCLEID");
            entity.Property(e => e.Itemname).HasMaxLength(100).HasColumnName("ITEMNAME");
            entity.Property(e => e.Quantity).HasMaxLength(50).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Cycleitems)
                .HasForeignKey(d => d.Cycleid)
                .HasConstraintName("FK__CYCLEITEM__CYCLE__571DF1D5");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Departmentid).HasName("PK__DEPARTME__EC3B6F4F6E908773");
            entity.ToTable("DEPARTMENTS");
            entity.Property(e => e.Departmentid).HasColumnName("DEPARTMENTID");
            entity.Property(e => e.Departmentname).HasMaxLength(255).HasColumnName("DEPARTMENTNAME");
            entity.Property(e => e.Universityid).HasColumnName("UNIVERSITYID");

            entity.HasOne(d => d.University).WithMany(p => p.Departments)
                .HasForeignKey(d => d.Universityid)
                .HasConstraintName("FK__DEPARTMEN__UNIVE__3C69FB99");
        });

        modelBuilder.Entity<Distribution>(entity =>
        {
            entity.HasKey(e => e.Distributionid).HasName("PK__DISTRIBU__A949078FBD87D6B8");
            entity.ToTable("DISTRIBUTIONS");
            entity.Property(e => e.Distributionid).HasColumnName("DISTRIBUTIONID");
            entity.Property(e => e.Cycleid).HasColumnName("CYCLEID");

            // REMOVED HasMaxLength
            entity.Property(e => e.Distributiondate).HasColumnName("DISTRIBUTIONDATE");

            entity.Property(e => e.Distributiontime).HasMaxLength(255).HasColumnName("DISTRIBUTIONTIME");
            entity.Property(e => e.Srcadminid).HasColumnName("SRCADMINID");
            entity.Property(e => e.Venueid).HasColumnName("VENUEID");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Distributions)
                .HasForeignKey(d => d.Cycleid)
                .HasConstraintName("FK__DISTRIBUT__CYCLE__693CA210");

            entity.HasOne(d => d.Srcadmin).WithMany(p => p.Distributions)
                .HasForeignKey(d => d.Srcadminid)
                .HasConstraintName("FK__DISTRIBUT__SRCAD__6B24EA82");

            entity.HasOne(d => d.Venue).WithMany(p => p.Distributions)
                .HasForeignKey(d => d.Venueid)
                .HasConstraintName("FK__DISTRIBUT__VENUE__6A30C649");
        });

        modelBuilder.Entity<Financestaff>(entity =>
        {
            entity.HasKey(e => e.Financestaffid).HasName("PK__FINANCES__60682FE2B2478997");
            entity.ToTable("FINANCESTAFF");
            entity.Property(e => e.Financestaffid).HasColumnName("FINANCESTAFFID");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Departmentid).HasColumnName("DEPARTMENTID");
            entity.Property(e => e.Personid).HasColumnName("PERSONID");

            entity.HasOne(d => d.Campus).WithMany(p => p.Financestaffs)
                .HasForeignKey(d => d.Campusid)
                .HasConstraintName("FK__FINANCEST__CAMPU__4BAC3F29");

            entity.HasOne(d => d.Department).WithMany(p => p.Financestaffs)
                .HasForeignKey(d => d.Departmentid)
                .HasConstraintName("FK__FINANCEST__DEPAR__4CA06362");

            entity.HasOne(d => d.Person).WithMany(p => p.Financestaffs)
                .HasForeignKey(d => d.Personid)
                .HasConstraintName("FK__FINANCEST__PERSO__4AB81AF0");
        });

        modelBuilder.Entity<Foodparcelcycle>(entity =>
        {
            entity.HasKey(e => e.Cycleid).HasName("PK__FOODPARC__23B576AD25C361A0");
            entity.ToTable("FOODPARCELCYCLES");
            entity.Property(e => e.Cycleid).HasColumnName("CYCLEID");
            entity.Property(e => e.Budget).HasColumnType("decimal(18, 2)").HasColumnName("BUDGET");
            entity.Property(e => e.Cyclename).HasMaxLength(255).HasColumnName("CYCLENAME");

            // REMOVED HasMaxLength
            entity.Property(e => e.Enddate).HasColumnName("ENDDATE");

            entity.Property(e => e.Maxbeneficiaries).HasColumnName("MAXBENEFICIARIES");
            entity.Property(e => e.Srcadminid).HasColumnName("SRCADMINID");

            // REMOVED HasMaxLength
            entity.Property(e => e.Startdate).HasColumnName("STARTDATE");

            entity.Property(e => e.Status).HasMaxLength(255).HasColumnName("STATUS");

            entity.HasOne(d => d.Srcadmin).WithMany(p => p.Foodparcelcycles)
                .HasForeignKey(d => d.Srcadminid)
                .HasConstraintName("FK__FOODPARCE__SRCAD__534D60F1");
        });

        modelBuilder.Entity<Fundingstatus>(entity =>
        {
            entity.HasKey(e => e.Fundingstatusid).HasName("PK__FUNDINGS__690AC5ED07FE2A9A");
            entity.ToTable("FUNDINGSTATUSES");
            entity.Property(e => e.Fundingstatusid).HasColumnName("FUNDINGSTATUSID");
            entity.Property(e => e.Financestaffid).HasColumnName("FINANCESTAFFID");
            entity.Property(e => e.Fundingstatus1).HasMaxLength(255).HasColumnName("FUNDINGSTATUS");
            entity.Property(e => e.Reason).HasMaxLength(255).HasColumnName("REASON");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");

            // REMOVED HasMaxLength
            entity.Property(e => e.Updateddate).HasColumnName("UPDATEDDATE");

            entity.HasOne(d => d.Financestaff).WithMany(p => p.Fundingstatuses)
                .HasForeignKey(d => d.Financestaffid)
                .HasConstraintName("FK__FUNDINGST__FINAN__66603565");

            entity.HasOne(d => d.Student).WithMany(p => p.Fundingstatuses)
                .HasForeignKey(d => d.Studentid)
                .HasConstraintName("FK__FUNDINGST__STUDE__656C112C");
        });

        modelBuilder.Entity<Itadmin>(entity =>
        {
            entity.HasKey(e => e.Itadminid).HasName("PK__ITADMINS__CB70B10A76A16960");
            entity.ToTable("ITADMINS");
            entity.Property(e => e.Itadminid).HasColumnName("ITADMINID");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Personid).HasColumnName("PERSONID");

            entity.HasOne(d => d.Campus).WithMany(p => p.Itadmins)
                .HasForeignKey(d => d.Campusid)
                .HasConstraintName("FK__ITADMINS__CAMPUS__47DBAE45");

            entity.HasOne(d => d.Person).WithMany(p => p.Itadmins)
                .HasForeignKey(d => d.Personid)
                .HasConstraintName("FK__ITADMINS__PERSON__46E78A0C");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Notificationid).HasName("PK__NOTIFICA__EAF93BF460CB9731");
            entity.ToTable("NOTIFICATIONS");
            entity.Property(e => e.Notificationid).HasColumnName("NOTIFICATIONID");

            // REMOVED HasMaxLength
            entity.Property(e => e.Createddate).HasColumnName("CREATEDDATE");

            entity.Property(e => e.Distributionid).HasColumnName("DISTRIBUTIONID");
            entity.Property(e => e.Isread).HasMaxLength(255).HasColumnName("ISREAD");
            entity.Property(e => e.Message).HasMaxLength(255).HasColumnName("MESSAGE");
            entity.Property(e => e.Notificationtype).HasMaxLength(255).HasColumnName("NOTIFICATIONTYPE");

            // REMOVED HasMaxLength
            entity.Property(e => e.Readdate).HasColumnName("READDATE");

            entity.Property(e => e.Srcadminid).HasColumnName("SRCADMINID");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("TITLE");

            entity.HasOne(d => d.Distribution).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Distributionid)
                .HasConstraintName("FK__NOTIFICAT__DISTR__6EF57B66");

            entity.HasOne(d => d.Srcadmin).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Srcadminid)
                .HasConstraintName("FK__NOTIFICAT__SRCAD__6FE99F9F");

            entity.HasOne(d => d.Student).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Studentid)
                .HasConstraintName("FK__NOTIFICAT__STUDE__6E01572D");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Personid).HasName("PK__PERSONS__0986239EB97F6256");
            entity.ToTable("PERSONS");
            entity.HasIndex(e => e.Username, "PERSONS_USERNAME_UK").IsUnique();
            entity.Property(e => e.Personid).HasColumnName("PERSONID");
            entity.Property(e => e.Accountstatus).HasMaxLength(255).HasColumnName("ACCOUNTSTATUS");

            // REMOVED HasMaxLength
            entity.Property(e => e.Createddate).HasColumnName("CREATEDDATE");
            entity.Property(e => e.Dateofbirth).HasColumnName("DATEOFBIRTH");

            entity.Property(e => e.Emailaddress).HasMaxLength(255).HasColumnName("EMAILADDRESS");
            entity.Property(e => e.Firstname).HasMaxLength(255).HasColumnName("FIRSTNAME");
            entity.Property(e => e.Gender).HasMaxLength(255).HasDefaultValue("Not Specified").HasColumnName("GENDER");

            // REMOVED HasMaxLength
            entity.Property(e => e.Lastlogin).HasColumnName("LASTLOGIN");

            entity.Property(e => e.Lastname).HasMaxLength(255).HasColumnName("LASTNAME");
            entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenumber).HasMaxLength(255).HasColumnName("PHONENUMBER");
            entity.Property(e => e.Username).HasMaxLength(255).HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("PK__REQUESTS__B4711F9AF3152643");
            entity.ToTable("REQUESTS");
            entity.HasIndex(e => new { e.Studentid, e.Cycleid }, "REQUESTS_CON_UNIQ").IsUnique();
            entity.Property(e => e.Requestid).HasColumnName("REQUESTID");
            entity.Property(e => e.Cycleid).HasColumnName("CYCLEID");

            // REMOVED HasMaxLength
            entity.Property(e => e.Requestdate).HasColumnName("REQUESTDATE");

            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Cycleid)
                .HasConstraintName("FK__REQUESTS__CYCLEI__5EBF139D");

            entity.HasOne(d => d.Student).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Studentid)
                .HasConstraintName("FK__REQUESTS__STUDEN__5DCAEF64");
        });

        modelBuilder.Entity<Srcadmin>(entity =>
        {
            entity.HasKey(e => e.Srcadminid).HasName("PK__SRCADMIN__C977B188EF0BE3F3");
            entity.ToTable("SRCADMINS");
            entity.Property(e => e.Srcadminid).HasColumnName("SRCADMINID");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Personid).HasColumnName("PERSONID");
            entity.Property(e => e.Termend).HasMaxLength(255).HasColumnName("TERMEND");
            entity.Property(e => e.Termstart).HasMaxLength(255).HasColumnName("TERMSTART");

            entity.HasOne(d => d.Campus).WithMany(p => p.Srcadmins)
                .HasForeignKey(d => d.Campusid)
                .HasConstraintName("FK__SRCADMINS__CAMPU__5070F446");

            entity.HasOne(d => d.Person).WithMany(p => p.Srcadmins)
                .HasForeignKey(d => d.Personid)
                .HasConstraintName("FK__SRCADMINS__PERSO__4F7CD00D");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Studentid).HasName("PK__STUDENTS__495196F0597276DF");
            entity.ToTable("STUDENTS");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Personid).HasColumnName("PERSONID");
            entity.Property(e => e.Studentnumber).HasMaxLength(255).HasColumnName("STUDENTNUMBER");

            entity.HasOne(d => d.Campus).WithMany(p => p.Students)
                .HasForeignKey(d => d.Campusid)
                .HasConstraintName("FK__STUDENTS__CAMPUS__4316F928");

            entity.HasOne(d => d.Person).WithMany(p => p.Students)
                .HasForeignKey(d => d.Personid)
                .HasConstraintName("FK__STUDENTS__PERSON__440B1D61");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.Universityid).HasName("PK__UNIVERSI__D1867B867AD2623A");
            entity.ToTable("UNIVERSITIES");
            entity.Property(e => e.Universityid).HasColumnName("UNIVERSITYID");
            entity.Property(e => e.Universityname).HasMaxLength(100).HasColumnName("UNIVERSITYNAME");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.Venueid).HasName("PK__VENUES__1D3FAAD1BE3C5941");
            entity.ToTable("VENUES");
            entity.Property(e => e.Venueid).HasColumnName("VENUEID");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Venuename).HasMaxLength(255).HasColumnName("VENUENAME");

            entity.HasOne(d => d.Campus).WithMany(p => p.Venues)
                .HasForeignKey(d => d.Campusid)
                .HasConstraintName("FK__VENUES__CAMPUSID__59FA5E80");
        });

        modelBuilder.Entity<VwApplicationOverview>(entity =>
        {
            entity.HasNoKey().ToView("VW_APPLICATION_OVERVIEW");
            entity.Property(e => e.Applicationstatus).HasMaxLength(255).HasColumnName("APPLICATIONSTATUS");
            entity.Property(e => e.Cyclename).HasMaxLength(255).HasColumnName("CYCLENAME");
            entity.Property(e => e.DecisionDate).HasMaxLength(255).HasColumnName("DECISION_DATE");
            entity.Property(e => e.DecisionReason).HasMaxLength(255).HasColumnName("DECISION_REASON");
            entity.Property(e => e.Enddate).HasMaxLength(255).HasColumnName("ENDDATE");
            entity.Property(e => e.Firstname).HasMaxLength(255).HasColumnName("FIRSTNAME");
            entity.Property(e => e.Lastname).HasMaxLength(255).HasColumnName("LASTNAME");
            entity.Property(e => e.Requestdate).HasMaxLength(255).HasColumnName("REQUESTDATE");
            entity.Property(e => e.Startdate).HasMaxLength(255).HasColumnName("STARTDATE");
            entity.Property(e => e.Studentnumber).HasMaxLength(255).HasColumnName("STUDENTNUMBER");
        });

        modelBuilder.Entity<VwStudentProfile>(entity =>
        {
            entity.HasNoKey().ToView("VW_STUDENT_PROFILE");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.Campusname).HasMaxLength(100).HasColumnName("CAMPUSNAME");
            entity.Property(e => e.Dateofbirth).HasMaxLength(255).HasColumnName("DATEOFBIRTH");
            entity.Property(e => e.Emailaddress).HasMaxLength(255).HasColumnName("EMAILADDRESS");
            entity.Property(e => e.Firstname).HasMaxLength(255).HasColumnName("FIRSTNAME");
            entity.Property(e => e.Gender).HasMaxLength(255).HasColumnName("GENDER");
            entity.Property(e => e.Lastname).HasMaxLength(255).HasColumnName("LASTNAME");
            entity.Property(e => e.Personid).HasColumnName("PERSONID");
            entity.Property(e => e.Phonenumber).HasMaxLength(255).HasColumnName("PHONENUMBER");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");
            entity.Property(e => e.Studentnumber).HasMaxLength(255).HasColumnName("STUDENTNUMBER");
            entity.Property(e => e.Universityid).HasColumnName("UNIVERSITYID");
            entity.Property(e => e.Universityname).HasMaxLength(100).HasColumnName("UNIVERSITYNAME");
            entity.Property(e => e.Username).HasMaxLength(255).HasColumnName("USERNAME");
        });

        modelBuilder.Entity<VwStudentRequest>(entity =>
        {
            entity.HasNoKey().ToView("VW_STUDENT_REQUESTS");
            entity.Property(e => e.CampusName).HasMaxLength(100).HasColumnName("CAMPUS NAME");
            entity.Property(e => e.Campusid).HasColumnName("CAMPUSID");
            entity.Property(e => e.CycleApplyingFor).HasMaxLength(255).HasColumnName("CYCLE APPLYING FOR");
            entity.Property(e => e.FirstName).HasMaxLength(255).HasColumnName("FIRST NAME");
            entity.Property(e => e.FundingStatus).HasMaxLength(255).HasColumnName("FUNDING STATUS");
            entity.Property(e => e.LastName).HasMaxLength(255).HasColumnName("LAST NAME");
            entity.Property(e => e.RequestDate).HasMaxLength(255).HasColumnName("REQUEST DATE");
            entity.Property(e => e.Requestid).HasColumnName("REQUESTID");
            entity.Property(e => e.StudentNumber).HasMaxLength(255).HasColumnName("STUDENT NUMBER");
            entity.Property(e => e.Studentid).HasColumnName("STUDENTID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}