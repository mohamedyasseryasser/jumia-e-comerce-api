using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Azure.Core.HttpHeader;

namespace e_comerce_api.models
{
    public class context:IdentityDbContext<applicationuser>
    {
     
        public  DbSet<address> Addresses { get; set; }
        public  DbSet<admin> Admins { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public  DbSet<AffiliateSellerRelationship> AffiliateSellerRelationships { get; set; }
        public  DbSet<Cart> Carts { get; set; }
        public  DbSet<CartItems> CartItems { get; set; }
        public  DbSet<category> Categories { get; set; }
        public  DbSet<Copons> Coupons { get; set; }
        public  DbSet<customer> Customers { get; set; }
        public  DbSet<HelpfulRating> HelpfulRatings { get; set; }
        public  DbSet<order> Orders { get; set; }
        public  DbSet<OrderItems> OrderItems { get; set; }
        public  DbSet<product> Products { get; set; }
         public DbSet<ProductAttributies> ProductAttributies { get; set; }

        public DbSet<ProductAttributiesValue> ProductAttributeValues { get; set; }
        public  DbSet<productVariant> ProductVariants { get; set; }
        public  DbSet<productview> ProductViews { get; set; }
        public  DbSet<Rating> Ratings { get; set; }
        public  DbSet<ReviewImage> ReviewImages { get; set; }
        public  DbSet<seller> Sellers { get; set; }
        public  DbSet<SubCategory> SubCategories { get; set; }
        public  DbSet<SubOrder> SubOrders { get; set; }
        public  DbSet<applicationuser> Users { get; set; }
        public  DbSet<UserCopon> UserCoupons { get; set; }
        public  DbSet<productVariantAttribute> VariantAttributes { get; set; }
        public  DbSet<WishList> Wishlists { get; set; }
        public  DbSet<WithItems> WishlistItems { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public context(DbContextOptions<context> option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            // حل مشكلة multiple cascade paths
            modelBuilder.Entity<HelpfulRating>()
                .HasOne(h => h.Rating)
                .WithMany(r => r.helpfulRating)
                .HasForeignKey(h => h.RatingId)
                .OnDelete(DeleteBehavior.Restrict);
            // تجاهل الـ ProducesAttribute
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.ProducesAttribute>();

            //deleted variant attributes
            modelBuilder.Entity<productVariantAttribute>()
       .HasOne(a => a.productVariant)
       .WithMany(v => v.attributes)
       .HasForeignKey(a => a.variatn_id)
       .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
