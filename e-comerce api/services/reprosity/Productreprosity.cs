using e_comerce_api.DTO;
using e_comerce_api.Enum;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace e_comerce_api.services.reprosity
{
    public class Productreprosity:Iproductreprosity
    {
        public Productreprosity(context context,Imagesreprosity imagesreprosity)
        {
            Context = context;
            Imagesreprosity = imagesreprosity;
        }
        public context Context { get; }
        public Imagesreprosity Imagesreprosity { get; }

        //get product by id
        public async Task<productresponsedto> getproductbyid(int productid,bool isincludedetails)
        {
            try
            {
                var exitproduct = await Context.Products.
                    Include(p=>p.seller).
                    ThenInclude(s=>s.user).
                    Include(p=>p.ratings).
                    Include(p=>p.ProductImage).
                    Include(p=>p.productVariants).
                    ThenInclude(v=>v.attributes).
                    Include(p=>p.ProductViews).
                    Include(p=>p.ProductAttributeValues).
                    ThenInclude(a=>a.Attribute).
                    Include(p=>p.subcategory).
                    ThenInclude(c=>c.category)
                    .FirstOrDefaultAsync(p => (p.ProductId==productid)
                    &&(p.isaveliable == true || isincludedetails == true));

                if (exitproduct == null)
                    throw new Exception("this product is not found");
                var result = mapresponseproduct(exitproduct,isincludedetails);
                if (result == null) throw new Exception("mapingresponse is null");

                return result;
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        //get all products
        public async Task<IEnumerable<productresponsedto>> getallproduct(PaginationDto paginationdto, Productfilterdto filter=null)
        {
            var totalitems = await Context.Products.CountAsync();
            var totalpages = (int)Math.Ceiling(totalitems / (double)paginationdto.pageSize);
            var products = Context.Products.
                Where(p=>p.isaveliable==true).
                AsQueryable();
            if (products == null)
            {
                throw new Exception("there are not products");
            }
            if (filter.CategoryId != null)
            {
                var exitcatid = await Context.Categories.FirstOrDefaultAsync(c => c.CategoryId == filter.CategoryId);
                if (exitcatid != null)
                {
                    products = products.Where(p => p.subcategory.category.CategoryId == filter.CategoryId);
                }
            }
            if (filter.SubcategoryId != null)
            {
                var exitsubcatid = await Context.SubCategories.FirstOrDefaultAsync(c => c.SbuCatId == filter.SubcategoryId);
                if (exitsubcatid != null)
                {
                    products = products.Where(p => p.subcategory.SbuCatId == filter.SubcategoryId);
                }
            }
            if (filter.MinPrice!=null&&filter.MaxPrice!=null)
            {
                products = products.Where(p=>p.basicprice<=filter.MaxPrice&&p.basicprice>=filter.MinPrice);
            }else if (filter.MaxPrice!=null)
            {
                products = products.Where(p=>p.basicprice<=filter.MaxPrice);
            }else if (filter.MinPrice!=null)
            {
                products = products.Where(p => p.basicprice >= filter.MinPrice);
            }
            products = ApplySorting(products,filter.SortBy,filter.SortDirection);
            var includedata = await products
              .Skip((paginationdto.pageSize) * paginationdto.pageNumber)
              .Take(paginationdto.pageSize)
              .Include(p => p.seller)
              .Include(p => p.subcategory).ThenInclude(s=>s.category)
              .Include(p => p.ratings)
              .Include(p => p.productVariants)
              .Include(p => p.ProductImage)
              .Include(p => p.ProductAttributeValues)
                  .ThenInclude(p => p.Attribute)
              .AsSplitQuery()
              .ToListAsync();
            return includedata.Select(i=> mapresponseproduct(i,true));
        }
        //create product
        public async Task<productresponsedto> createproduct(CreateProductInputDto dto,bool IsAdmin=false) {
            using var transaction =await Context.Database.BeginTransactionAsync();
            try
            {
                //create product
               
                product product = new product { 
                 basicprice=dto.BasePrice,
                 createdAt=DateTime.Now,
                 updatedAt=DateTime.Now,
                 discountpercentage=dto.DiscountPercentage,
                 FinalPrice=((dto.BasePrice)-(dto.BasePrice*dto.DiscountPercentage)/100),
                 Description=dto.Description,
                 isaveliable=true,
                 Name=dto.Name,
                 seller_id=dto.SellerId,
                 sub_cat_id=dto.SubcategoryId,
                 stockquantity=dto.StockQuantity,
                 AverageRate=0,
                }; 
                Context.Products.Add(product);
                await Context.SaveChangesAsync();
                //create images product 
                if (dto.MainImageFile != null)
                {
                    product.Mainimageurl = await Imagesreprosity.saveimageurl(dto.MainImageFile,EntityTyp.product,dto.Name);
                }
                
                if (dto.AdditionalImageFiles != null)
                {
                    foreach (var item in dto.AdditionalImageFiles)
                    {
                        var image = await Imagesreprosity.saveimageurl(item.Imagefile,EntityTyp.product,dto.Name);
                        var productimage = new ProductImage
                        {
                            DisplayOrder = item.DisplayOrder,
                            ImageUrl = image,
                            ProductId=product.ProductId,
                        };
                        product.ProductImage.Add(productimage);
                    }
                }
                //create attribute values
                if (dto.AttributeValues!=null)
                {
                    foreach (var item in dto.AttributeValues)
                    {

                        //validate attribute exit or not
                        var exitattribute = await Context.ProductAttributies.
                             FirstOrDefaultAsync(t => t.ProductAttriid == item.AttributeId );
                        if (exitattribute == null)
                            throw new Exception("this attribute is not found please create new attribute");
                        var possiblevaluearray = exitattribute.PossibleValues.Split(',');
                        if (!possiblevaluearray.Contains(item.Value, StringComparer.OrdinalIgnoreCase))
                        {
                            throw new Exception($"Invalid value. Allowed values: {string.Join(", ", exitattribute.PossibleValues)}");
                        }
                      
                     //validate attribute value
                        var attributevalue = new ProductAttributiesValue { 
                         AttributeId= item.AttributeId,
                         ProductId= product.ProductId,
                         Value= item.Value
                        };
                       await Context.ProductAttributeValues.AddAsync(attributevalue);                       
                    }
                    await Context.SaveChangesAsync();
                    //add variant
                    if (dto.HasVariants&&dto.Variants!=null)
                    {
                        foreach( var item in dto.Variants)
                        {
                            var variant = new productVariant { 
                             isaveliable=true,
                             discountpercentage=item.discountpercentage,
                             Description=item.Description,
                             basicprice=item.basicprice,
                             FinalPrice=(item.basicprice-(item.basicprice*item.discountpercentage)/100),
                             isdefault=item.isdefalult,
                             Mainimageurl="",
                             stockquantity=item.stockquantity,
                             product_id=product.ProductId,
                             SKU=item.sku,
                             VariantName=item.VariantName,
                            };
                             //variant image 
                            if (item.Imagefile!=null)
                            {
                              var imageurl = await Imagesreprosity.saveimageurl(item.Imagefile,EntityTyp.product,item.VariantName);
                              variant.Mainimageurl = imageurl;
                            }
                            await Context.ProductVariants.AddAsync(variant);
                            await Context.SaveChangesAsync();
                            //variant attribute
                            if (item.productVariantAttributeDtos != null)
                            {
                                var variantattributes=await Context.VariantAttributes.ToListAsync();
                                foreach (var attr in item.productVariantAttributeDtos) {
                                    var exitattribute = variantattributes.
                                        FirstOrDefault(t=>t.ProductVariantAttrid==attr.VariantAttributeId);
                                    var possiblevaluearray = exitattribute.possiblevalue.Split(',');
                                    if (!possiblevaluearray.Contains(attr.AttributeValue, StringComparer.OrdinalIgnoreCase))
                                    {
                                        throw new Exception($"Invalid value. Allowed values: {string.Join(", ", exitattribute.possiblevalue)}");
                                    }
                                    var newvariantattribute = new productVariantAttribute {
                                        AttributeName=attr.AttributeName,
                                        AttributeValue=attr.AttributeValue,
                                        variatn_id=variant.ProductVariantId,  
                                    };
                                    await Context.VariantAttributes.AddAsync(newvariantattribute);
                                }
                                await Context.SaveChangesAsync();
                            }
                        }
                       
                    }
                }
                await transaction.CommitAsync();
                return await getproductbyid(product.ProductId,true);
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }     
        }
        //validate attributevalue
        
        //maping productdto
        public productresponsedto mapresponseproduct(product product,bool isincludedetails)
        {
            var result = new productresponsedto
            {
                Description = product.Description,
                DiscountPercentage=product.discountpercentage,
                BasePrice=product.basicprice,
                CategoryId=product.subcategory.category.CategoryId,
               FinalPrice=product.FinalPrice,            
               //  FinalPrice=(product.basicprice-((product.discountpercentage*product.basicprice)/100)),
                CreatedAt=product.createdAt,
                Name=product.Name,
                IsAvailable=product.isaveliable,
                SellerId=product.seller.Sellerid,
                AverageRating=product.AverageRate,
                ProductId=product.ProductId,
                UpdatedAt=product.updatedAt,
                SellerName=product.seller.user.UserName,
                SubcategoryId=product.subcategory.SbuCatId,
                StockQuantity=product.stockquantity,
                CategoryName=product.subcategory.category.Name,
                SubcategoryName=product.subcategory.Name,
                MainImageUrl=product.Mainimageurl,
                RatingCount=product.ratings.Count(),
                ReviewCount=product.ProductViews.Count(),
            };
            if (product.ratings == null)
            {
                result.RatingCount = 0;
            }
            if (product.ProductViews==null)
            {
                result.ReviewCount = 0;
            }
            if (isincludedetails)
            {
                if (product.ProductImage != null)
                {
                    result.Images = product.ProductImage.Select(i => new ProductImageDto
                    {
                        ImageId = i.ImageId,
                        ProductId = i.ProductId,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                    }).ToList();
                }
                if (product.ProductAttributeValues != null)
                {
                    result.AttributeValues = product.ProductAttributeValues.Select(a => new ProductAttributeValueDto
                    {
                        AttributeId = a.Attribute.ProductAttriid,
                        AttributeName = a.Attribute.AttributeName,
                        AttributeType = a.Attribute.Type,
                        ProductId = a.ProductId,
                        Value = a.Value,
                        ValueId = a.ProductAttrValueId,
                    }).ToList();
                }
                if (product.productVariants != null)
                {
                    result.Variants = product.productVariants.Select(v => new ProductVariantDto
                    {
                        
                        DiscountPercentage = v.discountpercentage,
                        IsDefault = v.isdefault,
                        IsAvailable = v.isaveliable,
                        Price = v.basicprice,
                        finalprice=v.FinalPrice,
                        ProductId = v.product_id,
                        StockQuantity = v.stockquantity,
                        VariantId = v.ProductVariantId,
                        VariantName = v.VariantName,
                        VariantImageUrl = v.Mainimageurl,
                        VariantAttributes = v.attributes.Select(t => new UpdateVariantAttributeDto
                        {
                            Name = t.AttributeName,
                            Value = t.AttributeValue,
                            VariantId = t.variatn_id
                        }).ToList(),
                    }).ToList();
                }
            }
            return result;
        }
        public IQueryable<product> ApplySorting(IQueryable<product> products,string sortby,sortdirection sortdirection)
        {
            bool? ascending = sortdirection==sortdirection.ASCENDING?true:false;
            if (ascending==true) {
                switch (sortby.ToLower())
                {
                    case "name":
                        products.OrderBy(p=>p.Name).ToList();
                        break;
                    case "finalprice":
                        products.OrderBy(p => p.FinalPrice).ToList();
                        break;
                    case "stockquantity":
                        products.OrderBy(p => p.stockquantity).ToList();
                        break;
                    case "isavaliable":
                        products.OrderBy(p => p.isaveliable).ToList();
                        break;
                    default:
                        break;
                }
            }
            if (ascending == false)
            {
                switch (sortby.ToLower())
                {
                    case "name":
                        products.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "finalprice":
                        products.OrderByDescending(p => p.FinalPrice).ToList();
                        break;
                    case "stockquantity":
                        products.OrderByDescending(p => p.stockquantity).ToList();
                        break;
                    case "isavaliable":
                        products.OrderByDescending(p => p.isaveliable).ToList();
                        break;
                    default:
                        break;
                }
            }
            return products;
        }
        //update product
        public async Task<bool> updateproduct(updateproductinputdto productdto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                //check of product
                var product = await Context.Products.
                Include(p => p.ProductImage).
                Include(p => p.productVariants).
                ThenInclude(v => v.attributes).
                Include(p => p.ProductAttributeValues).
                ThenInclude(a => a.Attribute)
                .FirstOrDefaultAsync(p => p.ProductId == productdto.productid);
                if (product == null)
                    throw new Exception("this product is not found");
                //update product information
                try
                {
                    product.Name = productdto.Name;
                    product.Description = productdto.Description;
                    product.basicprice = productdto.BasePrice;
                    product.updatedAt = DateTime.Now;
                    product.stockquantity = productdto.StockQuantity;
                    product.discountpercentage = productdto.DiscountPercentage;
                    Context.Products.Update(product);
                    //await Context.SaveChangesAsync();
                }
                catch
                {
                    throw new Exception("this data is not valid");
                }
                //update product mainimage
                try
                {
                    if (productdto.MainImageFile != null)
                    {
                        var newproductmainimageurlpath = await Imagesreprosity.UpdateImageAsync(productdto.MainImageFile, product.Mainimageurl, EntityTyp.product, product.Name);
                        product.Mainimageurl = newproductmainimageurlpath;
                    }
                }
                catch
                {
                    throw new Exception("there are not mainimageurl");
                }
                //add product image
                if (productdto.AdditionalImageFiles != null)
                {
                    foreach (var image in productdto.AdditionalImageFiles)
                    {
                        var imagepath = await Imagesreprosity.saveimageurl(image.Imagefile, EntityTyp.product, $"{product.Name} images:{image.DisplayOrder}");
                        var productimage = new ProductImage
                        {
                            DisplayOrder = image.DisplayOrder,
                            ImageUrl = imagepath,
                            ProductId = product.ProductId,
                        };
                        product.ProductImage.Add(productimage);
                    }
                }
                //update product image deleted
                if (productdto.productimagedeleted != null)
                {
                    var imagesToDelete = product.ProductImage
                        .Where(pi => productdto.productimagedeleted.Contains(pi.ImageId))
                        .ToList();

                    foreach (var image in imagesToDelete)
                    {
                        await Imagesreprosity.deleteimage(image.ImageUrl);
                        Context.ProductImages.Remove(image);
                    }
                }
                //product attribute values
                if (productdto.AttributeValues != null)
                {
                    //remove exit attribute values
                    Context.ProductAttributeValues.RemoveRange(product.ProductAttributeValues);
                    await Context.SaveChangesAsync();
                    //add new attribute values
                    foreach (var value in productdto.AttributeValues)
                    {
                        var newattributevalue = new ProductAttributiesValue
                        {
                            AttributeId = value.AttributeId,
                            ProductId = value.ProductId,
                            Value = value.Value
                        };
                        await Context.ProductAttributeValues.AddAsync(newattributevalue);
                    }
                }
                //product variant
                if (productdto.HasVariants&&productdto.Variants!=null)
                {
                    //delete variant that is not found from update
                    var exitvariantsid = product.productVariants.Select(v => v.ProductVariantId).ToList();
                    var allvariants = product.productVariants.ToList();
                    var variantesdtoids = productdto.Variants.Select(v => v.VariantId).ToList();

                    var variantremoved = product.productVariants.
                        Where(v => !variantesdtoids
                    .Contains((int)v.ProductVariantId)).ToList();
                    foreach (var item in variantremoved)
                    {
                        item.isaveliable = false;
                    }
                    await Context.SaveChangesAsync();

                    //update or create variants
                    foreach (var variant in productdto.Variants)
                    {
                        if (variant.VariantId.HasValue && exitvariantsid.Contains(variant.VariantId.Value))
                        {
                            //update variant
                            var exitvariant = allvariants.FirstOrDefault(v => v.ProductVariantId == variant.VariantId.Value);
                            if (exitvariant == null)
                                throw new Exception("this variant is not found");
                            exitvariant.VariantName = variant.VariantName;
                            exitvariant.basicprice = variant.Price;
                            exitvariant.discountpercentage = variant.DiscountPercentage;
                            exitvariant.FinalPrice = (variant.Price - (variant.DiscountPercentage * variant.Price) / 100);
                            exitvariant.stockquantity = variant.StockQuantity;
                            exitvariant.SKU = variant.Sku;
                            exitvariant.isdefault = variant.IsDefault;
                            exitvariant.isaveliable = true;

                            //variant mainimagen
                            if (variant.VariantImage != null)
                            {
                                var newvariantimage = await Imagesreprosity.UpdateImageAsync(variant.VariantImage, exitvariant.Mainimageurl, EntityTyp.variant, $"{exitvariant.VariantName}");
                                exitvariant.Mainimageurl = newvariantimage;
                            }
                            //update variant attribute
                            if (variant.VariantAttributes != null)
                            {
                                Context.VariantAttributes.RemoveRange(exitvariant.attributes);
                                foreach (var attr in variant.VariantAttributes)
                                {
                                    var attribute = new productVariantAttribute
                                    {
                                        AttributeName = attr.AttributeName,
                                        AttributeValue = attr.AttributeValue,
                                        variatn_id = attr.VariantId,
                                        possiblevalue = attr.AttributeValue,
                                    };
                                    Context.VariantAttributes.Add(attribute);
                                }
                            }
                        }
                        //create new variant
                        else
                        {

                            var addvariant = new productVariant
                            {
                                isaveliable = true,
                                discountpercentage = variant.DiscountPercentage,
                                Description = variant.Description,
                                basicprice = variant.Price,
                                FinalPrice = (variant.Price - (variant.Price * variant.DiscountPercentage) / 100),
                                isdefault = variant.IsDefault,
                                Mainimageurl = "",
                                stockquantity = variant.StockQuantity,
                                product_id = product.ProductId,
                                SKU = variant.Sku,
                                VariantName = variant.VariantName,
                            };
                            //variant image 
                            if (variant.VariantImage != null)
                            {
                                var imageurl = await Imagesreprosity.saveimageurl(variant.VariantImage, EntityTyp.product, variant.VariantName);
                                addvariant.Mainimageurl = imageurl;
                            }
                            await Context.ProductVariants.AddAsync(addvariant);
                            await Context.SaveChangesAsync();
                            //variant attribute
                            if (variant.VariantAttributes != null)
                            {
                                var variantattributes = await Context.VariantAttributes.ToListAsync();
                                foreach (var attr in variant.VariantAttributes)
                                {
                                    var exitattribute = variantattributes.
                                        FirstOrDefault(t => t.ProductVariantAttrid == attr.VariantAttributeId);
                                    var possiblevaluearray = exitattribute.possiblevalue.Split(',');
                                    if (!possiblevaluearray.Contains(attr.AttributeValue, StringComparer.OrdinalIgnoreCase))
                                    {
                                        throw new Exception($"Invalid value. Allowed values: {string.Join(", ", exitattribute.possiblevalue)}");
                                    }
                                    var newvariantattribute = new productVariantAttribute
                                    {
                                        AttributeName = attr.AttributeName,
                                        AttributeValue = attr.AttributeValue,
                                        variatn_id = variant.VariantId,
                                    };
                                    await Context.VariantAttributes.AddAsync(newvariantattribute);
                                }
                                await Context.SaveChangesAsync();
                            }

                        }
                    }
                    //ensure is only one is default
                    var variantes = product.productVariants.Where(v => v.isdefault == true).ToList();

                    if (variantes.Count() > 1)
                    {
                        foreach (var item in variantes.Skip(1))
                        {
                            item.isdefault = false;
                        }
                    }
                }
                //if !hasvariant and product has variant
              
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //update product avaliability
        public async Task<bool> updateproductavaliablity(int productid, bool isavaliable)
        {
            try {
                var exitproduct = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == productid);
                if (exitproduct == null)
                    throw new Exception("this product is not found");
                exitproduct.isaveliable = isavaliable;
                await Context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
                throw;
            }
        }
        //delete product
        public async Task<bool> deletproduct(int productid)
        {
            using var transaction =await Context.Database.BeginTransactionAsync();
            try
            {
                var exitproduct = await Context.Products.FirstOrDefaultAsync(p => p.ProductId == productid);
                if (exitproduct == null)
                    throw new Exception("this product is not found");

                exitproduct.isaveliable = false;
                exitproduct.updatedAt= DateTime.Now;
                foreach (var variant in exitproduct.productVariants)
                {
                    variant.isaveliable = false;
                }
                //remove from cartitems
                var cartitems =await Context.CartItems.
                    Where(c=>c.ProductId==exitproduct.ProductId)
                    .ToListAsync();
                Context.CartItems.RemoveRange(cartitems);
                //remove wishlist
                var wishlistItems = await Context.WishlistItems
                    .Where(wi => wi.ProductId == exitproduct.ProductId)
                    .ToListAsync();
                Context.WishlistItems.RemoveRange(wishlistItems);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
                throw;
            }
        }
        //get product for specific seller
        public async Task<IEnumerable<productresponsedto>> getsellerproduct(int sellerid,PaginationDto paginationdto,Productfilterdto filter=null)
        {
            try
            {
                var totalitems = await Context.Products.CountAsync();
                var totalpages = (int)Math.Ceiling(totalitems / (double)paginationdto.pageSize);
                var products = Context.Products
                  .Where(p => p.seller_id == sellerid&&p.isaveliable==true);
                if (products == null)
                    throw new Exception("seller is not found");
                if (products == null)
                {
                    throw new Exception("there are not products");
                }
                if (filter.CategoryId != null)
                {
                    var exitcatid = await Context.Categories.FirstOrDefaultAsync(c => c.CategoryId == filter.CategoryId);
                    if (exitcatid != null)
                    {
                        products = products.Where(p => p.subcategory.category.CategoryId == filter.CategoryId);
                    }
                }
                if (filter.SubcategoryId != null)
                {
                    var exitsubcatid = await Context.SubCategories.FirstOrDefaultAsync(c => c.SbuCatId == filter.SubcategoryId);
                    if (exitsubcatid != null)
                    {
                        products = products.Where(p => p.subcategory.SbuCatId == filter.SubcategoryId);
                    }
                }
                products = ApplySorting(products, filter.SortBy, filter.SortDirection);
                var query = await products
               .Skip(paginationdto.pageSize * paginationdto.pageNumber)
               .Take(paginationdto.pageSize)
               .Include(p => p.subcategory)
               .ToListAsync();

                return products.Select(p => mapresponseproduct(p, false));
            }
            catch (Exception)
            {
                throw;
            }
        }
        //add product variant
        public async Task<productvariantresponsedto> addproductvariant(CreateProductVariantDto variant)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var exitproduct = await Context.Products.FirstOrDefaultAsync(p=>p.ProductId==variant.ProductId);
                if (exitproduct == null)
                    throw new Exception("this product is not found");
                var addvariant = new productVariant
                {
                    isaveliable =variant.IsAvailable||variant.StockQuantity>0?true:false,
                    discountpercentage = variant.DiscountPercentage,
                    Description = variant.description,
                    basicprice = variant.Price,
                    FinalPrice = variant.finalprice,
                    isdefault = variant.IsDefault,
                    Mainimageurl = "",
                    stockquantity = variant.StockQuantity,
                    product_id = exitproduct.ProductId,
                    SKU = variant.Sku,
                    VariantName = variant.VariantName,
                };
                //variant image 
                if (variant.VariantImageFile != null)
                {
                    var imageurl =await Imagesreprosity.saveimageurl(variant.VariantImageFile, EntityTyp.product, variant.VariantName);
                    addvariant.Mainimageurl = imageurl;

                }
                await Context.ProductVariants.AddAsync(addvariant);
                await Context.SaveChangesAsync();
                //variant attribute
                if (variant.VariantAttributes != null)
                {
                    foreach (var attr in variant.VariantAttributes)
                    {

                        var newvariantattribute = new productVariantAttribute
                        {
                            AttributeName = attr.AttributeName,
                            AttributeValue = attr.AttributeValue,
                            variatn_id = addvariant.ProductVariantId,
                        };
                        await Context.VariantAttributes.AddAsync(newvariantattribute);
                    }
                    await Context.SaveChangesAsync();
                }
                    if (variant.IsDefault==true)
                {                    
                        var variants =await Context.ProductVariants.
                            Where(v=>v.product_id==variant.ProductId&&v.ProductVariantId!=addvariant.ProductVariantId).
                            ToListAsync();
                        foreach (var item in variants)
                        {
                            item.isdefault = false;
                        }
                        await Context.SaveChangesAsync();
                }

                    await transaction.CommitAsync();
                return  mapproductvariant(addvariant);
                }

            catch (Exception)
            {
               await transaction.RollbackAsync();
                throw;
            }
        }
        //update product variant
        public async Task<productvariantresponsedto> updateproductvariant(UpdateProductBaseVariantDto variant)
        {
            var exitvariant = await Context.ProductVariants.Include(v=>v.attributes).
                FirstOrDefaultAsync(v=>v.ProductVariantId==variant.VariantId);
            if (exitvariant == null)
                throw new Exception($"{exitvariant.VariantName} is not found");
            //update variant
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                exitvariant.VariantName = variant.VariantName;
                exitvariant.basicprice = variant.Price;
                exitvariant.discountpercentage = variant.DiscountPercentage;
                exitvariant.FinalPrice = CalculateFinalPrice(variant.Price,variant.DiscountPercentage);
                exitvariant.stockquantity = variant.StockQuantity;
                exitvariant.SKU = variant.Sku;
                exitvariant.isdefault = variant.IsDefault;
                //avaliability product
                var product = await Context.Products.FirstOrDefaultAsync(p=>p.ProductId==exitvariant.product_id);
                exitvariant.isaveliable =product.isaveliable==true&&variant.StockQuantity>0 ;
                await Context.SaveChangesAsync();
                //variant mainimagen
                if (variant.VariantImage != null)
                {
                    var newvariantimage = await Imagesreprosity.UpdateImageAsync(variant.VariantImage, exitvariant.Mainimageurl, EntityTyp.variant, $"{exitvariant.VariantName}");
                    exitvariant.Mainimageurl = newvariantimage;
                }
                //update variant attribute
                if (variant.VariantAttributes != null)
                {
                    Context.VariantAttributes.RemoveRange(exitvariant.attributes);
                    await Context.SaveChangesAsync();
                    foreach (var attr in variant.VariantAttributes)
                    {
                        var attribute = new productVariantAttribute
                        {
                            AttributeName = attr.AttributeName,
                            AttributeValue = attr.AttributeValue,
                            variatn_id = attr.VariantId,
                            possiblevalue = attr.AttributeValue,
                        };
                        Context.VariantAttributes.Add(attribute);
                    }
                    await Context.SaveChangesAsync();
                }
                // If this is set as default, update other variants
                if (variant.IsDefault == true)
                {
                    var otherVariants = await  Context.ProductVariants
                        .Where(v => v.product_id == exitvariant.product_id 
                        && v.ProductVariantId != exitvariant.ProductVariantId)
                        .ToListAsync();

                    foreach (var otherVariant in otherVariants)
                    {
                        otherVariant.isdefault = false;
                    }

                    await Context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
               // await Context.SaveChangesAsync();
                return mapproductvariant(exitvariant);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //delete product variant
        public async Task<bool> deleteproductvariant(int variantid)
        {
            var variant = await Context.ProductVariants.
                Include(v=>v.attributes)
                .FirstOrDefaultAsync(v=>v.ProductVariantId==variantid);
            if (variant == null)
                throw new Exception($"{variantid} is not found");
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                //if variant is only one
                var product = Context.Products.Include(p=>p.productVariants).FirstOrDefault(p=>p.productVariants.Contains(variant));
                var countvariants = product.productVariants.Count();
                if (countvariants<=1)
                {
                    throw new InvalidOperationException("Cannot delete the only variant of a product");
                }
                //if this is default variant
                if (variant.isdefault==true)
                {
                    var newDefaultVariant = await Context.ProductVariants
                       .FirstOrDefaultAsync(v => v.product_id == variant.product_id && v.ProductVariantId != variantid);

                    if (newDefaultVariant != null)
                    {
                        newDefaultVariant.isdefault = true;
                        await Context.SaveChangesAsync();
                    }
                }
                // Delete variant attributes
                Context.VariantAttributes.RemoveRange(variant.attributes);

                // Delete the variant
                Context.ProductVariants.Remove(variant);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }

            catch(Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        // Update product main image
        public async Task<productresponsedto> UpdateProductMainImageAsync(int id, string imagePath)
        {
            var product = await Context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            // Delete old image if exists
            if (!string.IsNullOrEmpty(product.Mainimageurl))
            {
                await Imagesreprosity.deleteimage(product.Mainimageurl);
            }

            // Update image path
            product.Mainimageurl = imagePath;
            await Context.SaveChangesAsync();

            return await getproductbyid(id, false);
        }

        //map product variant
        public productvariantresponsedto mapproductvariant(productVariant variant)
        {
            if (variant == null)
                return null;
            var dtoresponse = new productvariantresponsedto {
                VariantId = variant.ProductVariantId,
                ProductId = variant.product_id,
                VariantName = variant.VariantName,
                Price = variant.basicprice,
                DiscountPercentage = variant.discountpercentage ,
                finalprice = CalculateFinalPrice(variant.basicprice, variant.discountpercentage),
                StockQuantity = variant.stockquantity,
                Sku = variant.SKU,
                VariantImageUrl = variant.Mainimageurl,
                IsDefault = variant.isdefault ?? false,
                IsAvailable = variant.isaveliable ?? false,
                VariantAttributes = variant.attributes?.Select(va => new ProductVariantAttributeDto
                {
                    VariantAttributeId = va.ProductVariantAttrid,
                    VariantId = variant.ProductVariantId,
                    AttributeName = va.AttributeName,
                    AttributeValue = va.AttributeValue
                }).ToList()
            };
            return dtoresponse;
        }
        //calcluate final price
        private decimal CalculateFinalPrice(decimal basePrice, decimal? discountPercentage)
        {
            if (!discountPercentage.HasValue || discountPercentage <= 0)
                return basePrice;

            var discountAmount = basePrice * (discountPercentage.Value / 100);
            return Math.Round(basePrice - discountAmount, 2);
        }
    }
}




















 