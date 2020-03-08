using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class CurrencyRateContext : DbContext
    {
        void ConfigureCurrencyRate(EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.ToTable(nameof(CurrencyRate));

            builder.Property(cr => cr.Id)
                .ForSqlServerUseSequenceHiLo("currency_rate_hilo")
                .IsRequired();

            builder.Property(cr => cr.DateTime)
                .IsRequired()
                .HasDefaultValueSql("current_timestamp");

            builder.Property(cr => cr.Currency)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(cr => cr.Rate)
                .IsRequired();

            // Индекс для выбора полной истории курсов валюты.
            builder.HasIndex(cr => cr.Currency);
            // Индекс для выбора курсов, ограниченных 24 часами.
            builder.HasIndex(cr => new { cr.DateTime, cr.Currency });
        }

        public CurrencyRateContext(DbContextOptions<CurrencyRateContext> options) : base(options) { }

        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>(ConfigureCurrencyRate);
        }
    }
}
