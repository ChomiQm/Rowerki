using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.ResourceManager.AppService.Models;
using System.Security;
using Project1.Configurations;
namespace Project1.Models;

public partial class ModelContext : DbContext
{
    public ModelContext() { }
   
    public ModelContext(DbContextOptions<ModelContext> options) : base(options) { }

    public virtual DbSet<BikeDepot> BikeDepots { get; set; }

    public virtual DbSet<CustOrdersDepartment> CustOrdersDepartments { get; set; }

    public virtual DbSet<CustomerDatum> CustomerData { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EngineRoomDepartment> EngineRoomDepartments { get; set; }

    public virtual DbSet<ManufacturingDepartment> ManufacturingDepartments { get; set; }

    public virtual DbSet<PaintRoomDepartment> PaintRoomDepartments { get; set; }

    public virtual DbSet<PartsDepot> PartsDepots { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Purpose> Purposes { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        string connectionString = KeyVaultImplementation.GetSecretFromKeyVault("ConnectionStrings--pysiec");
        if (connectionString == null)
        {
            throw new Exception("Unable to retrieve the connection string from Key Vault");
        }
        builder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BikeDepot>(entity =>
        {
            entity.HasKey(e => e.BikeId).HasName("PK__bike_dep__AD5B3CB798AA8E0D");

            entity.ToTable("bike_depot");

            entity.Property(e => e.BikeId).HasColumnName("bike_id");
            entity.Property(e => e.BikeDescription)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("bike_description");
            entity.Property(e => e.BikeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bike_name");
            entity.Property(e => e.DateOfStore)
                .HasColumnType("date")
                .HasColumnName("date_of_store");
            entity.Property(e => e.ManuDepSeriesId).HasColumnName("manu_dep_series_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ManuDepSeries).WithMany(p => p.BikeDepots)
                .HasForeignKey(d => d.ManuDepSeriesId)
                .HasConstraintName("FK__bike_depo__manu___6CD828CA");
        });

        modelBuilder.Entity<CustOrdersDepartment>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__cust_ord__46596229EBE49126");

            entity.ToTable("cust_orders_department");

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("order_id");
            entity.Property(e => e.BikeId).HasColumnName("bike_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("o_status");
            entity.Property(e => e.ShipmentId).HasColumnName("shipment_ID");
            entity.Property(e => e.Total).HasColumnName("total");

            entity.HasOne(d => d.Bike).WithMany(p => p.CustOrdersDepartments)
                .HasForeignKey(d => d.BikeId)
                .HasConstraintName("FK__cust_orde__bike___7755B73D");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustOrdersDepartments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__cust_orde__custo__76619304");

            entity.HasOne(d => d.Shipment).WithMany(p => p.CustOrdersDepartments)
                .HasForeignKey(d => d.ShipmentId)
                .HasConstraintName("FK__cust_orde__shipm__7849DB76");
        });

        modelBuilder.Entity<CustomerDatum>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB856662F198");

            entity.ToTable("customer_data");

            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("customer_id");
            entity.Property(e => e.CustomerLogin)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("customer_login");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerPassword)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("customer_password");
            entity.Property(e => e.CustomerSurrname)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("customer_surrname");
            entity.Property(e => e.DateOfFirstBuy)
                .HasColumnType("date")
                .HasColumnName("date_of_first_buy");
            entity.Property(e => e.Estabilishment)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("estabilishment");
            entity.Property(e => e.Mail)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("mail");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("street");
            entity.Property(e => e.Town)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("town");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA843692357");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeeSurrname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("employee_surrname");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("street");
            entity.Property(e => e.Town)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("town");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__employees__posit__56E8E7AB");
        });

        modelBuilder.Entity<EngineRoomDepartment>(entity =>
        {
            entity.HasKey(e => e.SeriesEngId).HasName("PK__engine_r__CCD1D324C24609EE");

            entity.ToTable("engine_room_departments");

            entity.Property(e => e.SeriesEngId).HasColumnName("series_eng_id");
            entity.Property(e => e.DateOfProduction)
                .HasColumnType("date")
                .HasColumnName("date_of_production");
            entity.Property(e => e.IdEngineroom).HasColumnName("ID_engineroom");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SeriesDescription)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("series_description");
            entity.Property(e => e.SeriesName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("series_name");
        });

        modelBuilder.Entity<ManufacturingDepartment>(entity =>
        {
            entity.HasKey(e => e.ManuSeriesId).HasName("PK__manufact__8ACBBB0358789AC6");

            entity.ToTable("manufacturing_department");

            entity.Property(e => e.ManuSeriesId).HasColumnName("manu_series_id");
            entity.Property(e => e.DateOfProduction)
                .HasColumnType("date")
                .HasColumnName("date_of_production");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<PaintRoomDepartment>(entity =>
        {
            entity.HasKey(e => e.SeriesPtnId).HasName("PK__paint_ro__949037B130C22A82");

            entity.ToTable("paint_room_departments");

            entity.Property(e => e.SeriesPtnId).HasColumnName("series_ptn_id");
            entity.Property(e => e.DateOfProduction)
                .HasColumnType("date")
                .HasColumnName("date_of_production");
            entity.Property(e => e.IdPaintroom).HasColumnName("ID_paintroom");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SeriesDescription)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("series_description");
            entity.Property(e => e.SeriesName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("series_name");
        });

        modelBuilder.Entity<PartsDepot>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__parts_de__A0E3FAB8B19A6D8D");

            entity.ToTable("parts_depot");

            entity.Property(e => e.PartId).HasColumnName("part_id");
            entity.Property(e => e.ManuDepSeriesId).HasColumnName("manu_dep_series_id");
            entity.Property(e => e.PartDescription)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("part_description");
            entity.Property(e => e.PartName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("part_name");
            entity.Property(e => e.PurposeId).HasColumnName("purpose_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ManuDepSeries).WithMany(p => p.PartsDepots)
                .HasForeignKey(d => d.ManuDepSeriesId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__parts_dep__manu___625A9A57");

            entity.HasOne(d => d.Purpose).WithMany(p => p.PartsDepots)
                .HasForeignKey(d => d.PurposeId)
                .HasConstraintName("FK__parts_dep__purpo__6166761E");

            entity.HasMany(d => d.SeriesEngs).WithMany(p => p.Parts)
                .UsingEntity<Dictionary<string, object>>(
                    "PartsEngineroom",
                    r => r.HasOne<EngineRoomDepartment>().WithMany()
                        .HasForeignKey("SeriesEngId")
                        .HasConstraintName("FK__parts_eng__serie__662B2B3B"),
                    l => l.HasOne<PartsDepot>().WithMany()
                        .HasForeignKey("PartId")
                        .HasConstraintName("FK__parts_eng__part___65370702"),
                    j =>
                    {
                        j.HasKey("PartId", "SeriesEngId").HasName("junction_engine_id");
                        j.ToTable("parts_engineroom");
                    });

            entity.HasMany(d => d.SeriesPtns).WithMany(p => p.Parts)
                .UsingEntity<Dictionary<string, object>>(
                    "PartsPaintroom",
                    r => r.HasOne<PaintRoomDepartment>().WithMany()
                        .HasForeignKey("SeriesPtnId")
                        .HasConstraintName("FK__parts_pai__serie__69FBBC1F"),
                    l => l.HasOne<PartsDepot>().WithMany()
                        .HasForeignKey("PartId")
                        .HasConstraintName("FK__parts_pai__part___690797E6"),
                    j =>
                    {
                        j.HasKey("PartId", "SeriesPtnId").HasName("junction_paint_id");
                        j.ToTable("parts_paintroom");
                    });
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__position__99A0E7A4895E7BE5");

            entity.ToTable("positions");

            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.PositionDescription)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("position_description");
            entity.Property(e => e.PositionName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("position_name");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("salary");
        });

        modelBuilder.Entity<Purpose>(entity =>
        {
            entity.HasKey(e => e.PurposeId).HasName("PK__purpose__8ED1FDD8BDB38A41");

            entity.ToTable("purpose");

            entity.Property(e => e.PurposeId).HasColumnName("purpose_id");
            entity.Property(e => e.PurposeName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("purpose_name");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__shipment__41416BC1A18DC968");

            entity.ToTable("shipment");

            entity.Property(e => e.ShipmentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("shipment_ID");
            entity.Property(e => e.DateOfReceipt)
                .HasColumnType("date")
                .HasColumnName("date_of_receipt");
            entity.Property(e => e.DateOfSent)
                .HasColumnType("date")
                .HasColumnName("date_of_sent");
            entity.Property(e => e.DeliveryType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("delivery_type");
            entity.Property(e => e.Destination)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("destination");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
