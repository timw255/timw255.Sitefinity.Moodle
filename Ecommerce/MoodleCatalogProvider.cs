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
using Telerik.Sitefinity.Modules.Ecommerce.Catalog;

namespace timw255.Sitefinity.Moodle.Ecommerce
{
    public class MoodleCatalogDataProvider : CatalogDataProviderBase
    {

        public override LanguageData CreateLanguageData(Guid id)
        {
            throw new NotImplementedException();
        }

        public override LanguageData CreateLanguageData()
        {
            throw new NotImplementedException();
        }

        public override Product CreateProduct(string productTypeName, Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override Product CreateProduct(string productTypeName)
        {
            throw new NotImplementedException();
        }

        public override ProductAttribute CreateProductAttribute(Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override ProductAttribute CreateProductAttribute()
        {
            throw new NotImplementedException();
        }

        public override ProductAttributeValue CreateProductAttributeValue(Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override ProductAttributeValue CreateProductAttributeValue()
        {
            throw new NotImplementedException();
        }

        public override ProductPrice CreateProductPrice(Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override ProductPrice CreateProductPrice()
        {
            throw new NotImplementedException();
        }

        public override ProductRating CreateProductRating(Guid id)
        {
            throw new NotImplementedException();
        }

        public override ProductRating CreateProductRating()
        {
            throw new NotImplementedException();
        }

        public override ProductVariation CreateProductVariation(Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override ProductVariation CreateProductVariation()
        {
            throw new NotImplementedException();
        }

        public override ProductVariationDetail CreateProductVariationDetail(Guid id, string applicationName)
        {
            throw new NotImplementedException();
        }

        public override ProductVariationDetail CreateProductVariationDetail()
        {
            throw new NotImplementedException();
        }

        public override void Delete(Product product)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductAttribute(ProductAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductAttributeLink(ProductAttributeLink attributeLink)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductAttributeValue(ProductAttributeValue value)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductPrice(ProductPrice price)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductRating(ProductRating productRating)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductVariation(ProductVariation productVariation)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProductVariationDetail(ProductVariationDetail productVariationDetail)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable GetItemsByTaxon(Guid taxonId, bool isSingleTaxon, string propertyName, Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<LanguageData> GetLanguageData()
        {
            throw new NotImplementedException();
        }

        public override LanguageData GetLanguageData(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Product GetProduct(string sku)
        {
            throw new NotImplementedException();
        }

        public override Product GetProduct(string sku, ContentLifecycleStatus status)
        {
            throw new NotImplementedException();
        }

        public override Product GetProduct(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override ProductAttribute GetProductAttribute(Guid id)
        {
            throw new NotImplementedException();
        }

        public override ProductAttribute GetProductAttributeByName(string name)
        {
            throw new NotImplementedException();
        }

        public override ProductAttributeLink GetProductAttributeLink(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductAttributeLink> GetProductAttributeLinks()
        {
            throw new NotImplementedException();
        }

        public override ProductAttributeValue GetProductAttributeValue(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductAttributeValue> GetProductAttributeValues(Guid parentId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductAttributeValue> GetProductAttributeValues()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductAttribute> GetProductAttributes()
        {
            throw new NotImplementedException();
        }

        public override ProductPrice GetProductPrice(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductPrice> GetProductPrices(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductPrice> GetProductPrices()
        {
            throw new NotImplementedException();
        }

        public override ProductRating GetProductRating(Guid productId, string language)
        {
            throw new NotImplementedException();
        }

        public override ProductRating GetProductRating(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductRating> GetProductRatings(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductRating> GetProductRatings()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductUrlData> GetProductUrlData()
        {
            throw new NotImplementedException();
        }

        public override ProductUrlData GetProductUrlData(Guid id)
        {
            throw new NotImplementedException();
        }

        public override ProductVariation GetProductVariation(Guid id)
        {
            throw new NotImplementedException();
        }

        public override ProductVariationDetail GetProductVariationDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductVariationDetail> GetProductVariationDetails(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductVariationDetail> GetProductVariationDetails()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductVariation> GetProductVariations(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<ProductVariation> GetProductVariations()
        {
            throw new NotImplementedException();
        }

        public override Product GetProductWithApplication(Guid productId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Product> GetProducts(string productClrType)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Product> GetProducts()
        {
            throw new NotImplementedException();
        }
    }
}
