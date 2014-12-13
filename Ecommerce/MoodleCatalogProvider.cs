using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Ecommerce.Catalog.Model;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog.Data;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.Ecommerce.Catalog.Web.Services;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Data.Linq;
using System.Reflection;
using Telerik.Sitefinity.Data.OA;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Exceptions;

namespace timw255.Sitefinity.Moodle.Ecommerce
{
    public class MoodleCatalogDataProvider : OpenAccessCatalogDataProvider
    {
        public OpenAccessProviderContext Context
        {
            get;
            set;
        }

        public MoodleCatalogDataProvider()
        {
        }

        public override LanguageData CreateLanguageData()
        {
            return this.CreateLanguageData(this.GetNewGuid());
        }

        public override LanguageData CreateLanguageData(Guid id)
        {
            LanguageData languageDatum = new LanguageData(this.ApplicationName, id);
            ((IDataItem)languageDatum).Provider = this;
            if (id != Guid.Empty)
            {
                this.GetContext().Add(languageDatum);
            }
            return languageDatum;
        }

        public override Product CreateProduct(string productTypeName)
        {
            if (string.IsNullOrEmpty(productTypeName))
            {
                throw new ArgumentNullException("productTypeName");
            }
            return this.CreateProduct(productTypeName, this.GetNewGuid(), this.ApplicationName);
        }

        public override Product CreateProduct(string productTypeName, Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(productTypeName))
            {
                throw new ArgumentNullException("productTypeName");
            }
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            IPersistentTypeDescriptor dynamicTypeDescriptor = this.GetDynamicTypeDescriptor(productTypeName);
            if (dynamicTypeDescriptor == null)
            {
                throw new ArgumentNullException(string.Concat("No persistent type with name ", productTypeName));
            }
            Product now = (Product)dynamicTypeDescriptor.CreateInstance(id);
            now.ApplicationName = applicationName;
            now.Provider = this;
            now.PublicationDate = DateTime.Now;
            now.LastModified = DateTime.Now;
            now.DateCreated = DateTime.Now;
            DateTime dateTime = DateTime.Now;
            now.ExpirationDate = new DateTime?(dateTime.AddYears(10));
            now.Owner = SecurityManager.GetCurrentUserId();
            now.OriginalOwner = SecurityManager.GetCurrentUserId();
            if (id != Guid.Empty)
            {
                this.GetContext().Add(now);
            }
            return now;
        }

        public override ProductAttribute CreateProductAttribute()
        {
            return this.CreateProductAttribute(this.GetNewGuid(), this.ApplicationName);
        }

        public override ProductAttribute CreateProductAttribute(Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            ProductAttribute productAttribute = new ProductAttribute(id, applicationName)
            {
                LastModified = DateTime.Now,
                DateCreated = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productAttribute);
            }
            return productAttribute;
        }

        public override ProductAttributeValue CreateProductAttributeValue()
        {
            return this.CreateProductAttributeValue(this.GetNewGuid(), this.ApplicationName);
        }

        public override ProductAttributeValue CreateProductAttributeValue(Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            ProductAttributeValue productAttributeValue = new ProductAttributeValue(id, applicationName)
            {
                LastModified = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productAttributeValue);
            }
            return productAttributeValue;
        }

        public override ProductPrice CreateProductPrice()
        {
            return this.CreateProductPrice(this.GetNewGuid(), this.ApplicationName);
        }

        public override ProductPrice CreateProductPrice(Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            ProductPrice productPrice = new ProductPrice(id, applicationName)
            {
                LastModified = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productPrice);
            }
            return productPrice;
        }

        public override ProductRating CreateProductRating()
        {
            return this.CreateProductRating(this.GetNewGuid());
        }

        public override ProductRating CreateProductRating(Guid id)
        {
            ProductRating productRating = new ProductRating(id, this.ApplicationName)
            {
                LastModified = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productRating);
            }
            return productRating;
        }

        public override ProductVariation CreateProductVariation()
        {
            return this.CreateProductVariation(this.GetNewGuid(), this.ApplicationName);
        }

        public override ProductVariation CreateProductVariation(Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            ProductVariation productVariation = new ProductVariation(id, applicationName)
            {
                LastModified = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productVariation);
            }
            return productVariation;
        }

        public override ProductVariationDetail CreateProductVariationDetail()
        {
            return this.CreateProductVariationDetail(this.GetNewGuid(), this.ApplicationName);
        }

        public override ProductVariationDetail CreateProductVariationDetail(Guid id, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = this.ApplicationName;
            }
            ProductVariationDetail productVariationDetail = new ProductVariationDetail(id, applicationName)
            {
                LastModified = DateTime.Now,
                Provider = this
            };
            if (id != Guid.Empty)
            {
                this.GetContext().Add(productVariationDetail);
            }
            return productVariationDetail;
        }

        public override void Delete(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }
            if (product.Status == ContentLifecycleStatus.Master)
            {
                Guid id = product.Id;
                IQueryable<Product> products =
                    from c in this.GetProducts()
                    where (int)c.Status != 0 && (c.OriginalContentId == id)
                    select c;
                foreach (Product product1 in products)
                {
                    ProductSynchronizer.RemoveContentLinkAssociations(product1.Id);
                    this.DeleteItem(product1);
                }
            }
            ProductSynchronizer.RemoveContentLinkAssociations(product.Id);
            foreach (ProductPrice tierPrice in product.TierPrices)
            {
                this.DeleteProductPrice(tierPrice);
            }
            this.DeleteItem(product);
        }

        public override void DeleteItem(object item)
        {
            if (typeof(Product).IsAssignableFrom(item.GetType()))
            {
                this.Delete((Product)item);
                return;
            }
            if (typeof(ProductVariation).IsAssignableFrom(item.GetType()))
            {
                this.DeleteProductVariation((ProductVariation)item);
                return;
            }
            if (typeof(ProductVariationDetail).IsAssignableFrom(item.GetType()))
            {
                this.DeleteProductVariationDetail((ProductVariationDetail)item);
                return;
            }
            if (typeof(ProductAttribute).IsAssignableFrom(item.GetType()))
            {
                this.DeleteProductAttribute((ProductAttribute)item);
                return;
            }
            if (typeof(ProductAttributeValue).IsAssignableFrom(item.GetType()))
            {
                this.DeleteProductAttributeValue((ProductAttributeValue)item);
                return;
            }
            if (!typeof(ProductPrice).IsAssignableFrom(item.GetType()))
            {
                throw new UnsupportedException(string.Format("Unsupported object of type '{0}'", item.GetType().Name));
            }
            this.DeleteProductPrice((ProductPrice)item);
        }

        public override void DeleteProductAttribute(ProductAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            this.GetContext().Remove(attribute);
        }

        public override void DeleteProductAttributeLink(ProductAttributeLink attributeLink)
        {
            if (attributeLink == null)
            {
                throw new ArgumentNullException("attribute link cannot be null");
            }
            this.GetContext().Remove(attributeLink);
        }

        public override void DeleteProductAttributeValue(ProductAttributeValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.GetContext().Remove(value);
        }

        public override void DeleteProductPrice(ProductPrice price)
        {
            if (price == null)
            {
                throw new ArgumentNullException("productVariationDetail");
            }
            this.GetContext().Remove(price);
        }

        public override void DeleteProductRating(ProductRating rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException("The product rating cannot be null");
            }
            this.GetContext().Remove(rating);
        }

        public override void DeleteProductVariation(ProductVariation productVariation)
        {
            if (productVariation == null)
            {
                throw new ArgumentNullException("productVariation");
            }
            this.GetContext().Remove(productVariation);
        }

        public override void DeleteProductVariationDetail(ProductVariationDetail productVariationDetail)
        {
            if (productVariationDetail == null)
            {
                throw new ArgumentNullException("productVariationDetail");
            }
            this.GetContext().Remove(productVariationDetail);
        }

        public virtual new IEnumerable GetItems(Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            if (!typeof(Product).IsAssignableFrom(itemType))
            {
                throw DataProviderBase.GetInvalidItemTypeException(itemType, this.GetKnownTypes());
            }
            return DataProviderBase.SetExpressions<Product>(this.GetProducts(itemType.ToString()), filterExpression, orderExpression, new int?(skip), new int?(take), ref totalCount);
        }

        public override IEnumerable GetItemsByTaxon(Guid taxonId, bool isSingleTaxon, string propertyName, Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            if (!typeof(Product).IsAssignableFrom(itemType))
            {
                throw DataProviderBase.GetInvalidItemTypeException(itemType, new Type[] { typeof(Product) });
            }
            base.CurrentTaxonomyProperty = propertyName;
            int? nullable = null;
            IQueryable<Product> items = (IQueryable<Product>)this.GetItems(itemType, filterExpression, orderExpression, 0, 0, ref nullable);
            if (!isSingleTaxon)
            {
                IQueryable<Product> products =
                    from i in items
                    where i.GetValue<IList<Guid>>(this.CurrentTaxonomyProperty).Any<Guid>((Guid t) => t == taxonId)
                    select i;
                items = products;
            }
            else
            {
                IQueryable<Product> value =
                    from i in items
                    where i.GetValue<Guid>(this.CurrentTaxonomyProperty) == taxonId
                    select i;
                items = value;
            }
            if (totalCount.HasValue)
            {
                totalCount = new int?(items.Count<Product>());
            }
            if (skip > 0)
            {
                items = items.Skip<Product>(skip);
            }
            if (take > 0)
            {
                items = items.Take<Product>(take);
            }
            return items;
        }

        public override LanguageData GetLanguageData(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Argument 'id' cannot be empty GUID.");
            }
            LanguageData itemById = this.GetContext().GetItemById<LanguageData>(id.ToString());
            ((IDataItem)itemById).Provider = this;
            return itemById;
        }

        public override IQueryable<LanguageData> GetLanguageData()
        {
            string applicationName = this.ApplicationName;
            return
                from c in SitefinityQuery.Get<LanguageData>(this)
                where c.ApplicationName == applicationName
                select c;
        }

        public override Product GetProduct(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Argument 'productId' cannot be empty GUID.");
            }
            Product itemById = this.GetContext().GetItemById<Product>(productId.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override Product GetProduct(string sku, ContentLifecycleStatus status)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ArgumentException("Argument 'sku' cannot be empty.");
            }
            Product product = (
                from p in SitefinityQuery.Get<Product>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Sku == sku) && (int)p.Status == (int)status
                select p).SingleOrDefault<Product>();
            if (product != null)
            {
                product.Provider = this;
            }
            return product;
        }

        public override Product GetProduct(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ArgumentException("Argument 'sku' cannot be empty.");
            }
            Product product = (
                from p in SitefinityQuery.Get<Product>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Sku == sku) && (int)p.Status == 2
                select p).SingleOrDefault<Product>();
            if (product != null)
            {
                product.Provider = this;
            }
            return product;
        }

        public override ProductAttribute GetProductAttribute(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductAttribute itemById = this.GetContext().GetItemById<ProductAttribute>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override ProductAttribute GetProductAttributeByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            return (
                from p in SitefinityQuery.Get<ProductAttribute>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Name == name)
                select p).FirstOrDefault<ProductAttribute>();
        }

        public override ProductAttributeLink GetProductAttributeLink(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Argument 'id' cannot be empty GUID.");
            }
            ProductAttributeLink itemById = this.GetContext().GetItemById<ProductAttributeLink>(id.ToString());
            return itemById;
        }

        public override IQueryable<ProductAttributeLink> GetProductAttributeLinks()
        {
            return SitefinityQuery.Get<ProductAttributeLink>(this, MethodBase.GetCurrentMethod());
        }

        public override IQueryable<ProductAttribute> GetProductAttributes()
        {
            return
                from p in SitefinityQuery.Get<ProductAttribute>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override ProductAttributeValue GetProductAttributeValue(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductAttributeValue itemById = this.GetContext().GetItemById<ProductAttributeValue>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override IQueryable<ProductAttributeValue> GetProductAttributeValues()
        {
            return
                from p in SitefinityQuery.Get<ProductAttributeValue>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<ProductAttributeValue> GetProductAttributeValues(Guid parentId)
        {
            return
                from p in SitefinityQuery.Get<ProductAttributeValue>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Parent.Id == parentId)
                select p;
        }

        public override ProductPrice GetProductPrice(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductPrice itemById = this.GetContext().GetItemById<ProductPrice>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override IQueryable<ProductPrice> GetProductPrices()
        {
            return
                from p in SitefinityQuery.Get<ProductPrice>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<ProductPrice> GetProductPrices(Guid productId)
        {
            return
                from p in SitefinityQuery.Get<ProductPrice>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override ProductRating GetProductRating(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductRating itemById = this.GetContext().GetItemById<ProductRating>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override ProductRating GetProductRating(Guid productId, string language)
        {
            if (productId == Guid.Empty)
            {
                return null;
            }
            ProductRating productRating = (
                from p in SitefinityQuery.Get<ProductRating>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.ParentId == productId) && (p.Language == language)
                select p).Single<ProductRating>();
            productRating.Provider = this;
            return productRating;
        }

        public override IQueryable<ProductRating> GetProductRatings()
        {
            return
                from p in SitefinityQuery.Get<ProductRating>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<ProductRating> GetProductRatings(Guid productId)
        {
            return
                from p in SitefinityQuery.Get<ProductRating>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<Product> GetProducts()
        {
            return
                from p in SitefinityQuery.Get<Product>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<Product> GetProducts(string productClrType)
        {
            Type describedType = this.GetDynamicTypeDescriptor(productClrType).DescribedType;
            return
                from p in SitefinityQuery.Get<Product>(describedType, this)
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override ProductUrlData GetProductUrlData(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Argument 'id' cannot be empty GUID.");
            }
            ProductUrlData itemById = this.GetContext().GetItemById<ProductUrlData>(id.ToString());
            ((IDataItem)itemById).Provider = this;
            return itemById;
        }

        public override IQueryable<ProductUrlData> GetProductUrlData()
        {
            return
                from p in SitefinityQuery.Get<ProductUrlData>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override ProductVariation GetProductVariation(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductVariation itemById = this.GetContext().GetItemById<ProductVariation>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override ProductVariationDetail GetProductVariationDetail(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            ProductVariationDetail itemById = this.GetContext().GetItemById<ProductVariationDetail>(id.ToString());
            itemById.Provider = this;
            return itemById;
        }

        public override IQueryable<ProductVariationDetail> GetProductVariationDetails()
        {
            return
                from p in SitefinityQuery.Get<ProductVariationDetail>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<ProductVariationDetail> GetProductVariationDetails(Guid productId)
        {
            return
                from p in SitefinityQuery.Get<ProductVariationDetail>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Parent.Id == productId)
                select p;
        }

        public override IQueryable<ProductVariation> GetProductVariations()
        {
            return
                from p in SitefinityQuery.Get<ProductVariation>(this, MethodBase.GetCurrentMethod())
                where p.ApplicationName == this.ApplicationName
                select p;
        }

        public override IQueryable<ProductVariation> GetProductVariations(Guid productId)
        {
            return
                from p in SitefinityQuery.Get<ProductVariation>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Parent.Id == productId)
                select p;
        }

        public override Product GetProductWithApplication(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Argument 'productId' cannot be empty GUID.");
            }
            Product product = (
                from p in SitefinityQuery.Get<Product>(this, MethodBase.GetCurrentMethod())
                where (p.ApplicationName == this.ApplicationName) && (p.Id == productId)
                select p).SingleOrDefault<Product>();
            if (product == null)
            {
                return null;
            }
            product.Provider = this;
            return product;
        }
    }
}
