using Microsoft.EntityFrameworkCore;

namespace ProjectDataAccess.DbModel
{
    public class Risk_DbContext:DbContext
    {
        public Risk_DbContext(DbContextOptions<Risk_DbContext> options):base(options) { }
        public DbSet<DataContainer> DataContainer { get; set; }
        public DbSet<Data_Control> Data_Control { get; set; }
        public DbSet<Data_Version> Data_Version { get; set; }
        public DbSet<DataRisk> DataRisk { get; set; }
        public DbSet<ContainerID> ContainerID { get; set; }
        public DbSet<DataContainerID> DataContainerID { get; set; }
        public DbSet<v_DataContainer> v_DataContainer { get; set; }
        public DbSet<v_DataContainer_CFG> v_DataContainer_CFG { get; set; }
        public DbSet<v_DataContainer_VER> v_DataContainer_VER { get; set; }
        public DbSet<v_DataRisk> v_DataRisk { get; set; }
        public DbSet<v_DataRisk_CFG> v_DataRisk_CFG { get; set; }
        public DbSet<v_ContainerID> v_ContainerID { get; set; }
        public DbSet<v_DataContainerID> v_DataContainerID { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataContainer>().HasKey(op => new { op.DataContainerID, op.id_version });
            modelBuilder.Entity<Data_Control>().HasKey(op => new { op.DataContainerId, op.id_version });
            modelBuilder.Entity<Data_Version>().HasKey(op => new { op.DataContainerId, op.id_version });
            modelBuilder.Entity<DataRisk>().HasKey(op => new { op.DataContainerID, op.id_version, op.RiskID });
            modelBuilder.Entity<ContainerID>().HasKey(op => new { op.ContainerId});
            modelBuilder.Entity<DataContainerID>().HasKey(op => new { op.dataContainerID });

            modelBuilder.Entity<v_DataContainer>(op => { op.HasNoKey(); op.ToView("v_DataContainer"); });
            modelBuilder.Entity<v_DataContainer_CFG>(op => { op.HasNoKey(); op.ToView("v_DataContainer_CFG"); });
            modelBuilder.Entity<v_DataContainer_VER>(op => { op.HasNoKey(); op.ToView("v_DataContainer_VER"); });
            modelBuilder.Entity<v_DataRisk>(op => { op.HasNoKey(); op.ToView("v_DataRisk"); });
            modelBuilder.Entity<v_DataRisk_CFG>(op => { op.HasNoKey(); op.ToView("v_DataRisk_CFG"); });
            modelBuilder.Entity<v_ContainerID>(op => { op.HasNoKey(); op.ToView("v_ContainerID"); });
            modelBuilder.Entity<v_DataContainerID>(op => { op.HasNoKey(); op.ToView("v_DataContainerID"); });
        }
    }
}
