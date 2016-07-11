using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.ThemeHelper.Computer
{
    public class ComputerPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<Product> _productRepository;

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWidgetService _widgetService;

        private readonly LocalizationSettings _localizationSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly WidgetSettings _widgetSettings;
        private readonly MediaSettings _mediaSettings;

        private readonly IWebHelper _webHelper;

        #endregion

        #region Utilities

        private void AddProductTag(Product product, string tag)
        {
            var productTag = _productTagRepository.Table.FirstOrDefault(pt => pt.Name == tag);
            if (productTag == null)
            {
                productTag = new ProductTag
                {
                    Name = tag,
                };
            }
            product.ProductTags.Add(productTag);
            _productRepository.Update(product);
        }

        private void InstallSpecificationAttributes()
        {
            var sa1 = new SpecificationAttribute
            {
                Name = "Color",
                DisplayOrder = 0,
            };
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "White",
                DisplayOrder = 1,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Black",
                DisplayOrder = 2,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Red",
                DisplayOrder = 3
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Blue",
                DisplayOrder = 4,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Gray",
                DisplayOrder = 5,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Pink",
                DisplayOrder = 6,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "Champagne",
                DisplayOrder = 7,
            });
            sa1.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "White/Black",
                DisplayOrder = 8,
            });

            var sa2 = new SpecificationAttribute
            {
                Name = "RAM",
                DisplayOrder = 1,
            };
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "512 MB",
                DisplayOrder = 0,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "1 GB",
                DisplayOrder = 1,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "4 GB",
                DisplayOrder = 2,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "6 GB",
                DisplayOrder = 3,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "8 GB",
                DisplayOrder = 4,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "12 GB",
                DisplayOrder = 5,
            });
            sa2.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "16 GB",
                DisplayOrder = 6,
            });

            var sa3 = new SpecificationAttribute
            {
                Name = "HDD",
                DisplayOrder = 2,
            };
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "8 GB",
                DisplayOrder = 1,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "16 GB",
                DisplayOrder = 2,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "32 GB",
                DisplayOrder = 3,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "320 GB",
                DisplayOrder = 4,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "500 GB",
                DisplayOrder = 5,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "750 GB",
                DisplayOrder = 6,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "1 TB",
                DisplayOrder = 7,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "2 TB",
                DisplayOrder = 8,
            });
            sa3.SpecificationAttributeOptions.Add(new SpecificationAttributeOption
            {
                Name = "4 TB",
                DisplayOrder = 9,
            });

            var specificationAttributes = new List<SpecificationAttribute>
                                {
                                    sa1,
                                    sa2,
                                    sa3
                                };
            specificationAttributes.ForEach(sa => _specificationAttributeRepository.Insert(sa));

        }

        private void InstallBlogPosts()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var blogPosts = new List<BlogPost>
                                {
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Online Discount Coupons",
                                             Body = "<p>Online discount coupons enable access to great offers from some of the world&rsquo;s best sites for Internet shopping. The online coupons are designed to allow compulsive online shoppers to access massive discounts on a variety of products. The regular shopper accesses the coupons in bulk and avails of great festive offers and freebies thrown in from time to time.  The coupon code option is most commonly used when using a shopping cart. The coupon code is entered on the order page just before checking out. Every online shopping resource has a discount coupon submission option to confirm the coupon code. The dedicated web sites allow the shopper to check whether or not a discount is still applicable. If it is, the sites also enable the shopper to calculate the total cost after deducting the coupon amount like in the case of grocery coupons.  Online discount coupons are very convenient to use. They offer great deals and professionally negotiated rates if bought from special online coupon outlets. With a little research and at times, insider knowledge the online discount coupons are a real steal. They are designed to promote products by offering &lsquo;real value for money&rsquo; packages. The coupons are legitimate and help with budgeting, in the case of a compulsive shopper. They are available for special trade show promotions, nightlife, sporting events and dinner shows and just about anything that could be associated with the promotion of a product. The coupons enable the online shopper to optimize net access more effectively. Getting a &lsquo;big deal&rsquo; is not more utopian amidst rising prices. The online coupons offer internet access to the best and cheapest products displayed online. Big discounts are only a code away! By Gaynor Borade (buzzle.com)</p>",
                                             Tags = "e-commerce, money",
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new BlogPost
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "Customer Service - Client Service",
                                             Body = "<p>Managing online business requires different skills and abilities than managing a business in the &lsquo;real world.&rsquo; Customers can easily detect the size and determine the prestige of a business when they have the ability to walk in and take a look around. Not only do &lsquo;real-world&rsquo; furnishings and location tell the customer what level of professionalism to expect, but &quot;real world&quot; personal encounters allow first impressions to be determined by how the business approaches its customer service. When a customer walks into a retail business just about anywhere in the world, that customer expects prompt and personal service, especially with regards to questions that they may have about products they wish to purchase.<br /><br />Customer service or the client service is the service provided to the customer for his satisfaction during and after the purchase. It is necessary to every business organization to understand the customer needs for value added service. So customer data collection is essential. For this, a good customer service is important. The easiest way to lose a client is because of the poor customer service. The importance of customer service changes by product, industry and customer. Client service is an important part of every business organization. Each organization is different in its attitude towards customer service. Customer service requires a superior quality service through a careful design and execution of a series of activities which include people, technology and processes. Good customer service starts with the design and communication between the company and the staff.<br /><br />In some ways, the lack of a physical business location allows the online business some leeway that their &lsquo;real world&rsquo; counterparts do not enjoy. Location is not important, furnishings are not an issue, and most of the visual first impression is made through the professional design of the business website.<br /><br />However, one thing still remains true. Customers will make their first impressions on the customer service they encounter. Unfortunately, in online business there is no opportunity for front- line staff to make a good impression. Every interaction the customer has with the website will be their primary means of making their first impression towards the business and its client service. Good customer service in any online business is a direct result of good website design and planning.</p><p>By Jayashree Pakhare (buzzle.com)</p>",
                                             Tags = "e-commerce, nopCommerce, asp.net, sample tag, money",
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            blogPosts.ForEach(bp => _blogPostRepository.Insert(bp));

            //search engine names
            foreach (var blogPost in blogPosts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = blogPost.Id,
                    EntityName = "BlogPost",
                    LanguageId = blogPost.LanguageId,
                    IsActive = true,
                    Slug = blogPost.ValidateSeName("", blogPost.Title, true)
                });
            }
        }

        private void InstallNews()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var news = new List<NewsItem>
                                {
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "nopCommerce new release!",
                                             Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!<br /><br />nopCommerce is a fully customizable shopping cart. It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
                                             Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p><p>For full feature list go to <a href=\"http://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "New online store is open!",
                                             Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.",
                                             Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            news.ForEach(n => _newsItemRepository.Insert(n));

            //search engine names
            foreach (var newsItem in news)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = newsItem.Id,
                    EntityName = "NewsItem",
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = newsItem.ValidateSeName("", newsItem.Title, true)
                });
            }
        }

        private void InstallCategories()
        {
            var sampleImagesPath = CommonHelper.MapPath("~/Themes/Computer/Content/images/SampleData/");
            var pictureId = 0;

            var categoryTemplateInGridAndLines =
                _categoryTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines") ??
                _categoryTemplateRepository.Table.FirstOrDefault();

            var allCategories = new List<Category>();

            var categoryDesktops = new Category
            {
                Name = "Desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Desktops",
                MetaDescription = "Desktops",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-desktops.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Desktops")).Id,
                PriceRanges = "-600;600-1500;1500-;",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryDesktops);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "desktops-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Desktops")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryDesktops, "category-icon", pictureId);

            allCategories.Add(categoryDesktops);

            var categoryAllinone = new Category
            {
                Name = "All-in-one",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "All-in-one",
                MetaDescription = "All-in-one",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-allinone.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("All-in-one")).Id,
                PriceRanges = "-500;500-1500;1500-;",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ParentCategoryId = categoryDesktops.Id,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryAllinone);
            allCategories.Add(categoryAllinone);

            var categoryEveryday = new Category
            {
                Name = "Everyday desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Everyday desktops",
                MetaDescription = "Everyday desktops",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-everyday-desktops.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Everyday desktops")).Id,
                PriceRanges = "-500;500-1500;1500-;",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ParentCategoryId = categoryDesktops.Id,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryEveryday);
            allCategories.Add(categoryEveryday);

            var categoryMultimedia = new Category
            {
                Name = "Multimedia & Entertainment",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Multimedia, Entertainment",
                MetaDescription = "Multimedia & Entertainment",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-multimedia.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Multimedia & Entertainment")).Id,
                PriceRanges = "-500;500-1500;1500-;",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ParentCategoryId = categoryDesktops.Id,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryMultimedia);
            allCategories.Add(categoryMultimedia);

            var categoryGaming = new Category
            {
                Name = "Gaming",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Gaming",
                MetaDescription = "Gaming",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-gaming.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Gaming")).Id,
                PriceRanges = "-500;500-1500;1500-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ParentCategoryId = categoryDesktops.Id,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryGaming);
            allCategories.Add(categoryGaming);

            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Notebooks",
                MetaDescription = "Notebooks",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-notebooks.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Notebooks")).Id,
                PriceRanges = "-500;500-1500;1500-;",
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryNotebooks);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "notebooks-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Notebooks")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryNotebooks, "category-icon", pictureId);

            allCategories.Add(categoryNotebooks);

            var categoryTablets = new Category
            {
                Name = "Tablets",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Tablets",
                MetaDescription = "Tablets",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-tablets.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Tablets")).Id,
                PriceRanges = "-100;100-200;200-;",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryTablets);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "tablets-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Tablets")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryTablets, "category-icon", pictureId);

            allCategories.Add(categoryTablets);


            var categoryMonitors = new Category
            {
                Name = "Monitors",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Monitors",
                MetaDescription = "Monitors",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "3, 6, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-monitors.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Monitors")).Id,
                PriceRanges = "-1000;1000-2000;2000-;",
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryMonitors);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "monitors-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Monitors")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryMonitors, "category-icon", pictureId);

            allCategories.Add(categoryMonitors);


            var categoryAccessories = new Category
            {
                Name = "Accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Accessories",
                MetaDescription = "Accessories",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-accessories.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Accessories")).Id,
                PriceRanges = "-200;200-500;500-;",
                Published = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryAccessories);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "accessories-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Accessories")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryAccessories, "category-icon", pictureId);

            allCategories.Add(categoryAccessories);


            var categoryNetwork = new Category
            {
                Name = "Network",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Network",
                MetaDescription = "Network",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                PictureId =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "category-network.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName("Network")).Id,
                PriceRanges = "-100;100-500;500-;",
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IncludeInTopMenu = true,
            };
            _categoryRepository.Insert(categoryNetwork);

            //add category icon
            pictureId =
                _pictureService.InsertPicture(
                    File.ReadAllBytes(sampleImagesPath +
                                      "network-icon.png"), "image/png",
                    _pictureService.GetPictureSeName("Network")).Id;
            _genericAttributeService.SaveAttribute<int>(categoryNetwork, "category-icon", pictureId);

            allCategories.Add(categoryNetwork);

            //search engine names
            foreach (var category in allCategories)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = "Category",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = category.ValidateSeName("", category.Name, true)
                });
            }
        }

        private void InstallManufacturers()
        {
            var manufacturerTemplateInGridAndLines =
                _manufacturerTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines") ??
                _manufacturerTemplateRepository.Table.FirstOrDefault();

            var allManufacturers = new List<Manufacturer>();

            var manufacturerAsus = new Manufacturer
            {
                Name = "Asus",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerAsus);
            allManufacturers.Add(manufacturerAsus);

            var manufacturerAcer = new Manufacturer
            {
                Name = "Acer",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerAcer);
            allManufacturers.Add(manufacturerAcer);

            var manufacturerDell = new Manufacturer
            {
                Name = "Dell",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerDell);
            allManufacturers.Add(manufacturerDell);

            var manufacturerHp = new Manufacturer
            {
                Name = "HP",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerHp);
            allManufacturers.Add(manufacturerHp);

            var manufacturerLenovo = new Manufacturer
            {
                Name = "Lenovo",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerLenovo);
            allManufacturers.Add(manufacturerLenovo);

            var manufacturerToshiba = new Manufacturer
            {
                Name = "Toshiba",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerToshiba);
            allManufacturers.Add(manufacturerToshiba);

            var manufacturerSamsung = new Manufacturer
            {
                Name = "Samsung",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerSamsung);
            allManufacturers.Add(manufacturerSamsung);

            var manufacturerSony = new Manufacturer
            {
                Name = "Sony",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 12, 24",
                Published = true,
                DisplayOrder = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PriceRanges = "-500;500-1000;1000-"
            };
            _manufacturerRepository.Insert(manufacturerSony);
            allManufacturers.Add(manufacturerSony);

            //search engine names
            foreach (var manufacturer in allManufacturers)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = manufacturer.Id,
                    EntityName = "Manufacturer",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = manufacturer.ValidateSeName("", manufacturer.Name, true)
                });
            }
        }

        private void InstallProductAttributes()
        {
            var productAttributes = new List<ProductAttribute>
            {
                new ProductAttribute
                {
                    Name = "Color",
                },
                new ProductAttribute
                {
                    Name = "RAM",
                },
                new ProductAttribute
                {
                    Name = "HDD",
                },
                new ProductAttribute
                {
                    Name = "OS",
                },
                new ProductAttribute
                {
                    Name = "Software",
                }
            };
            productAttributes.ForEach(pa => _productAttributeRepository.Insert(pa));
        }

        private void InstallProducts()
        {
            //pictures
            var sampleImagesPath = CommonHelper.MapPath("~/Themes/Computer/Content/images/SampleData/");

            var productTemplateGrouped =
                _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)") ??
                _productTemplateRepository.Table.FirstOrDefault();
            var productTemplateSimple =
                _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product") ??
                _productTemplateRepository.Table.FirstOrDefault();

            var allProducts = new List<Product>();


            var productAceracerAz500 = new Product
            {
                Name = "Acer AZ5700-U2102",
                OldPrice = 1300M,
                Price = 1269M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Saves space in any home or office and is easy to use with interactive multi-gesture touch.",
                FullDescription = "<ul><li>23-inch touch-screen display; Saves space in any home or office and is easy to use with interactive multi-gesture touch computing, enhanced media sharing with social networks and stunning HD entertainment</li><li>Experience a faster, more intuitive and fun way of computing with the Acer all-in-one PC with easy-to-use touch screen technology, eye-catching style and fantastic entertainment and media sharing</li><li>View ultra-sharp, vibrant images on the 23-inch Full HD (16:9) widescreen touch-screen display, enhancing entertainment, graphic intensive creation, productivity applications, gaming, Internet surfing, digital TV and more</li><li>Space saving design integrates a PC and monitor into one; Adjustable back stand, 802.11b/g/n connectivity and Wireless Keyboard and Mouse keep things clean and organized while an illuminated red strip serves as a night light and adds elegance to any room</li><li>Windows 7 compliant multi-touch capability streamlines navigation with exciting, easy usability and hands-on efficiencies; Glide through multiple applications for a superior touch screen experience to enjoy your PC as a TV or media hub</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 5,
                Length = 19,
                Width = 35,
                Height = 31,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                MarkAsNew = true
            };
            allProducts.Add(productAceracerAz500);
            productAceracerAz500.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "All-in-one"),
                DisplayOrder = 1,
            });
            productAceracerAz500.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAceracerAz500.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Acer")
            });

            //pictures
            productAceracerAz500.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-acerAZ500-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAceracerAz500.Name)),
                DisplayOrder = 0,
            });
            productAceracerAz500.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-acerAZ500-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAceracerAz500.Name)),
                DisplayOrder = 1,
            });
            productAceracerAz500.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-acerAZ500-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAceracerAz500.Name)),
                DisplayOrder = 2,
            });

            productAceracerAz500.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "Color").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "Gray").FirstOrDefault()
            });
            productAceracerAz500.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "8 GB").FirstOrDefault()
            });
            productAceracerAz500.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "1 TB")
            });
            var pvaAceracerAz500 = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Software").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pvaAceracerAz500.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Microsoft Office",
                PriceAdjustment = 50,
                DisplayOrder = 1,
            });
            pvaAceracerAz500.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Acrobat Reader",
                PriceAdjustment = 10,
                DisplayOrder = 2,
            });
            pvaAceracerAz500.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Total Commander",
                PriceAdjustment = 5,
                DisplayOrder = 3,
            });
            productAceracerAz500.ProductAttributeMappings.Add(pvaAceracerAz500);

            _productRepository.Insert(productAceracerAz500);


            var productHpTouchSmart = new Product
            {
                Name = "HP TouchSmart 310-1126",
                OldPrice = 1390M,
                Price = 1360M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "HP TouchSmart 310-1126 has a little bit on 20 “HD widescreen LCD and comes with a built-in touch-optimized applications suite.",
                FullDescription = "<ul><li> 2.90 GHz AMD Athlon II Dual Core 245 </li><li> 6 GB DDR3 SDRAM </li><li> 1000 GB 7200 rpm hard drive</li><li> Windows 7 Home Premium 64-bit </li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 5,
                Length = 19,
                Width = 35,
                Height = 31,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                MarkAsNew = true
            };
            allProducts.Add(productHpTouchSmart);
            productHpTouchSmart.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "All-in-one"),
                DisplayOrder = 1,
            });
            productHpTouchSmart.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });
            productHpTouchSmart.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productHpTouchSmart.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Hp")
            });

            productHpTouchSmart.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "6 GB").FirstOrDefault()
            });
            productHpTouchSmart.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productHpTouchSmart.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "1 TB")
            });

            //pictures
            productHpTouchSmart.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-hptouchsmart3101126-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpTouchSmart.Name)),
                DisplayOrder = 0,
            });
            productHpTouchSmart.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-hptouchsmart3101126-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpTouchSmart.Name)),
                DisplayOrder = 1,
            });
            productHpTouchSmart.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-hptouchsmart3101126-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpTouchSmart.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productHpTouchSmart);


            var productToshibaQosmio = new Product
            {
                Name = "Toshiba Qosmio DX730",
                Price = 1399M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The Qosmio DX730 is Toshiba's first all-in-one PC, a stylish entertainment center.",
                FullDescription = "Intel Core i7 3610QM, 2300 МГц, 8192 Мб, 2000 Гб, GeForce GT 630M 2048 Мб, Blu-Ray, Wi-Fi, Bluetooth, 23'' Multi-Touch (1920x1080).",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 4,
                Length = 1,
                Width = 20,
                Height = 30,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                MarkAsNew = true
            };
            allProducts.Add(productToshibaQosmio);
            productToshibaQosmio.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "All-in-one"),
                DisplayOrder = 1,
            });
            productToshibaQosmio.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });
            productToshibaQosmio.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });

            //manufacturers
            productToshibaQosmio.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Toshiba")
            });

            productToshibaQosmio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "8 GB").FirstOrDefault()
            });
            productToshibaQosmio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            var pvaToshibaQosmio = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "HDD").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaToshibaQosmio.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "500 GB",
                DisplayOrder = 1,
            });
            pvaToshibaQosmio.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "1 TB",
                PriceAdjustment = 199,
                DisplayOrder = 2,
            });
            productToshibaQosmio.ProductAttributeMappings.Add(pvaToshibaQosmio);

            var pva2ToshibaQosmio = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "OS").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pva2ToshibaQosmio.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Home Basic",
                PriceAdjustment = 99,
                DisplayOrder = 1,
            });
            pva2ToshibaQosmio.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Home Premium",
                PriceAdjustment = 120,
                DisplayOrder = 2,
            });
            productToshibaQosmio.ProductAttributeMappings.Add(pva2ToshibaQosmio);

            //pictures
            productToshibaQosmio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "product-ToshibaQosmio-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productToshibaQosmio.Name)),
                DisplayOrder = 0,
            });
            productToshibaQosmio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "product-ToshibaQosmio-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productToshibaQosmio.Name)),
                DisplayOrder = 1,
            });
            productToshibaQosmio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                                    "product-ToshibaQosmio-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productToshibaQosmio.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productToshibaQosmio);

            //LENOVO K430
            var productLenovoK430 = new Product
            {
                Name = "Lenovo IdeaCentre K430",
                Price = 869M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The Lenovo IdeaCentre K430 with AOC e2343-F 23'' LED-Backlit Monitor is an incredible computer package deal that will really light up your home",
                FullDescription = "The Lenovo K430 is an excellent desktop that provides a powerful performance for all your computing tasks and multimedia. The processor, RAM and dedicated graphics memory combination make this desktop lightening fast and able to handle intensive tasks without a problem. Powered by the Intel Core i7 processor and 12GB RAM, you will experience super fast running speeds and excellent efficiency. Multi-task with ease and challenge your PC with demanding games and applications for an impressive performance. The Lenovo three-speed power control switch lets you change into turbo gear to rev up your gaming or speed up internet streaming. Then change down to conserve power, noise and heat emissions during light use such as email and web surfing. Store all your photos, video, music and more on the enormous 4TB hard drive. There is plenty of room for multiple users to store data for convenient access.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productLenovoK430);
            productLenovoK430.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Multimedia & Entertainment"),
                DisplayOrder = 1,
            });
            productLenovoK430.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });
            productLenovoK430.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productLenovoK430.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Lenovo")
            });

            productLenovoK430.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "12 GB").FirstOrDefault()
            });
            productLenovoK430.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productLenovoK430.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "4 TB")
            });

            productLenovoK430.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-lenovoK430-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoK430.Name)),
                DisplayOrder = 0,
            });
            productLenovoK430.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-lenovoK430-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoK430.Name)),
                DisplayOrder = 1,
            });
            productLenovoK430.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-lenovoK430-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoK430.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productLenovoK430);


            //Dell Inspiron 560
            var productDellInspiron560 = new Product
            {
                OldPrice = 600,
                Price = 578M,
                Name = "Dell Inspiron 560",
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Intel Pentium Dual-Core E5700 Processor, Windows 7 Home Premium (i560-3911NBK Black)",
                FullDescription = "The Dell Inspiron 560 desktop PC is everything you want in a desktop computer. Whether surfing the web, emailing friends and family, downloading music and photos or blogging about it all, the Dell Inspiron 560 desktop PC can handle it. </br></br> <b>Dell Inspiron 560 Desktop PC with Intel Pentium Dual-Core E5700 Processor with 20\" Monitor:</b> </br></br> <ul><li>Intel Pentium Dual-Core E5700 processor<br>3.0GHz, 800MHz Front Side Bus, 2MB L2 Cache<p></p></li><li>4GB DDR3 SDRAM system memory<br>Gives you the power to handle most power-hungry applications and tons of multimedia work<p></p></li><li>500GB SATA hard drive<br>Store 333,000 photos, 142,000 songs or 263 hours of HD video and more<p></p></li><li>16X Multi-format DVD Burner<br>Watch movies, and read and write CDs and DVDs in multiple formats<p></p></li><li>10/100 Ethernet<br>Connect to a broadband modem with or wired broadband router with wired Ethernet<p></p></li><li>20\" widescreen flat panel monitor<br>Intel Media Accelerator X4500HD Graphics</li></ul></br></br> <b>Additional Features:</b> </br><ul><li>19-in-1 memory card reader</li><li>6 x USB 2.0 ports, 1 x VGA port, 2 x microphone jacks, 1 x headphone jack, 1 x HDMI port, 1 x RJ-45 Ethernet port, 1 x line-out, 1 x line-in</li><li>1 x PCI Express (x16) slot, 1 x PCI Express (x1) slot, 2 x PCI Express (x2) slots, 2 x 5.25\" bays, 2 x 3.5\" bays</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productDellInspiron560);
            productDellInspiron560.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Multimedia & Entertainment"),
                DisplayOrder = 1,
            });
            productDellInspiron560.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });
            productDellInspiron560.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday desktops"),
                DisplayOrder = 1,
            });
            productDellInspiron560.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });

            //manufacturers
            productDellInspiron560.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Dell")
            });

            productDellInspiron560.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "4 GB").FirstOrDefault()
            });
            productDellInspiron560.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productDellInspiron560.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "500 GB")
            });

            productDellInspiron560.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-DellInspiron560-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productDellInspiron560.Name)),
                DisplayOrder = 0,
            });
            productDellInspiron560.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-DellInspiron560-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productDellInspiron560.Name)),
                DisplayOrder = 1,
            });
            _productRepository.Insert(productDellInspiron560);


            //Asus Essentio
            var productAsusEssentio = new Product
            {
                Name = "ASUS Essentio CM1630",
                Price = 799M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "This ASUS Essentio CM1630 is the perfect computer for those who demand powerful multimedia performance",
                FullDescription = "<strong> ASUS Essentio CM1630-05 Athlon II X2 2.8 GHz Desktop PC</strong><br><br> General Features: <ul></li><li>Microsoft Windows 7 Home Premium pre-installed w/CoA </li><li>AMD Athlon II X2 220 2.8 GHz dual-core processor </li><li>2 x 128  KB L1 cache, 2 x 512 KB L2 cache, 45 nm SOI CMOS </li><li>AMD 760G/SB710 chipset </li><li>4 GB DDR3 RAM (supports up to 16 GB) </li><li>750 GB SATA hard drive </li><li>DVD±RW DL drive </li><li>Integrated ATI Radeon 3000 graphics </li><li>Integrated ALC887 8-channel audio </li><li>Integrated 10/100/1000 Gigabit Ethernet LAN </li><li>No modem </li><li>Memory card reader <br><br></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAsusEssentio);
            productAsusEssentio.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Multimedia & Entertainment"),
                DisplayOrder = 1,
            });
            productAsusEssentio.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAsusEssentio.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Asus")
            });

            productAsusEssentio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productAsusEssentio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "4 GB")
            });
            productAsusEssentio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "750 GB")
            });

            productAsusEssentio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusEssentio-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusEssentio.Name)),
                DisplayOrder = 0,
            });
            productAsusEssentio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusEssentio-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusEssentio.Name)),
                DisplayOrder = 1,
            });
            productAsusEssentio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusEssentio-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusEssentio.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productAsusEssentio);


            //HP Pavilion p7-1380t 
            var productHpPavilionp7 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "HP Pavilion p7-1380t",
                Price = 599M,
                ShortDescription = "Powerful desktop with Intel quad-core processor is built to handle demanding digital needs with integrated graphics and beats audio.",
                FullDescription = "<ul><li>3rd Generation Intel(R) Core(TM) i5-3330 quad-core processor </li><li>Windows 8 or other operating systems available</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productHpPavilionp7);
            productHpPavilionp7.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday Desktops"),
                DisplayOrder = 1,
            });
            productHpPavilionp7.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productHpPavilionp7.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Hp")
            });

            var pvaHpPavilionp7 = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "RAM").Single(),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pvaHpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "2 GB",
                DisplayOrder = 1,
            });
            pvaHpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "4 GB",
                PriceAdjustment = 20,
                DisplayOrder = 2,
            });
            pvaHpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "8 GB",
                PriceAdjustment = 60,
                DisplayOrder = 3,
            });
            productHpPavilionp7.ProductAttributeMappings.Add(pvaHpPavilionp7);


            var pva2HpPavilionp7 = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Single(x => x.Name == "HDD"),
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true,
            };
            pva2HpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "320 GB",
                DisplayOrder = 1,
            });
            pva2HpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "500 GB",
                PriceAdjustment = 99,
                DisplayOrder = 2,
            });
            pva2HpPavilionp7.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "1 TB",
                PriceAdjustment = 199,
                DisplayOrder = 3,
            });
            productHpPavilionp7.ProductAttributeMappings.Add(pva2HpPavilionp7);

            productHpPavilionp7.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            productHpPavilionp7.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-HPPavillionP7-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpPavilionp7.Name)),
                DisplayOrder = 0,
            });
            productHpPavilionp7.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-HPPavillionP7-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpPavilionp7.Name)),
                DisplayOrder = 1,
            });
            productHpPavilionp7.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-HPPavillionP7-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productHpPavilionp7.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productHpPavilionp7);


            //Acer Predator G3
            var productAcerPredatorG3 = new Product
            {
                Name = "Acer Predator AG3610-UR30P",
                Price = 899M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The Acer Predator AG3610-UR30P is one gorgeous-looking and modern desktop.",
                FullDescription = "<ul><li>3.4GHz Intel Core i7-2600 Quad-Core</li><li>8GB of DDR3 RAM (2 x 4GB)</li><li>2TB Hard Drive (5400rpm)</li><li>nVIDIA GeForce GT 530 Graphics (2GB)</li><li>SuperMulti DVD Burner</li><li>Multi-in-1 Media Card Reader</li><li>High Definition Audio Support</li><li>802.11b/g/n Wi-Fi, Gigabit Ethernet</li><li>Included USB Keyboard and Optical Mouse</li><li>Windows 7 Home Premium (64-bit)</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAcerPredatorG3);
            productAcerPredatorG3.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday Desktops"),
                DisplayOrder = 1,
            });
            productAcerPredatorG3.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAcerPredatorG3.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Acer")
            });

            productAcerPredatorG3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productAcerPredatorG3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "8 GB")
            });
            productAcerPredatorG3.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "2 TB")
            });

            var pvaAcerPredatorG3 = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "OS").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pvaAcerPredatorG3.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Home Basic",
                PriceAdjustment = 99,
                DisplayOrder = 1,
            });
            pvaAcerPredatorG3.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Home Premium",
                PriceAdjustment = 120,
                DisplayOrder = 2,
            });
            productAcerPredatorG3.ProductAttributeMappings.Add(pvaAcerPredatorG3);

            var pva2AcerPredatorG3 = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "Software").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pva2AcerPredatorG3.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Microsoft Office",
                PriceAdjustment = 50,
                DisplayOrder = 1,
            });
            pva2AcerPredatorG3.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Acrobat Reader",
                PriceAdjustment = 10,
                DisplayOrder = 2,
            });
            pva2AcerPredatorG3.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Total Commander",
                PriceAdjustment = 5,
                DisplayOrder = 3,
            });
            productAcerPredatorG3.ProductAttributeMappings.Add(pva2AcerPredatorG3);

            productAcerPredatorG3.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerPredatorG3-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerPredatorG3.Name)),
                DisplayOrder = 0,
            });
            productAcerPredatorG3.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                        "product-AcerPredatorG3-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(
                            productAcerPredatorG3.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productAcerPredatorG3);


            //Cyberpower
            var productCyberpower = new Product
            {
                Name = "CyberPower Black Pearl",
                OldPrice = 4500M,
                Price = 4200M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Ultimate Gaming PC.",
                FullDescription = "An excellent RAM is a must if you are planning to buy a gaming pc. Black pearl has 12GB of DDR3 (Kingston) having a rate of 1600 MHz. Any gamer can use from 2GB to 24GB. This way there is tremendous RAM power for your pc. Although the secondary drive (7200 1TB hard drive) makes your pc slow. <br/> The motherboard can hold up to 3 cards for video and audio simultaneously. This means crisp, fast, ultra-superb audio and video dispensation. The PC uses GTX 580- the best card available in the market. There are 2 of these present. For any gamer the graphic is of real import- the GTX 580 is a part of DX 11 (a multimedia used to give a real feel to the games). <br />",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productCyberpower);
            productCyberpower.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });
            productCyberpower.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });

            productCyberpower.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "12 GB").FirstOrDefault()
            });
            productCyberpower.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productCyberpower.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "1 TB")
            });

            productCyberpower.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Cyberpower-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productCyberpower.Name)),
                DisplayOrder = 0,
            });
            productCyberpower.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Cyberpower-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productCyberpower.Name)),
                DisplayOrder = 1,
            });
            productCyberpower.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Cyberpower-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productCyberpower.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productCyberpower);


            //Maingear
            var productMaingear = new Product
            {
                Name = "MAINGEAR SHIFT Super Stock Gaming Desktop",
                OldPrice = 4999M,
                Price = 4200M,
                ProductType = ProductType.GroupedProduct,
                VisibleIndividually = true,
                ShortDescription = "The Shift S's mid-range hardware gives you enough power for current games at a good price.",
                FullDescription = "<ul><li>Core i7 3770K with MAINGEAR EPIC 180 closed-loop liquid cooling</li><li>16GB Corsair Vengeance DDR3-1600</li><li>120GB Corsair Neutron SSD SATA 6G with 555MB/s sustained read speed for OS + 2TB 7200 RPM SATA Seagate</li><li>NVIDIA GeForce GTX 680 (Kepler, 28nm) 2GB GDDR5 with PhysX, 3D Vision</li><li>Operating System Provided: Windows 8 64-bit</li></ul>",
                ProductTemplateId = productTemplateGrouped.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productMaingear);

            productMaingear.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });
            productMaingear.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Desktops"),
                DisplayOrder = 1,
            });
            productMaingear.ProductCategories.Add(new ProductCategory
            {
                Category = _categoryRepository.Table.FirstOrDefault(c => c.Name == "Everyday desktops"),
                DisplayOrder = 1,
            });

            productMaingear.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Red")
            });
            productMaingear.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Blue")
            });
            productMaingear.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "White")
            });
            productMaingear.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "16 GB").FirstOrDefault()
            });
            productMaingear.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "2 TB").FirstOrDefault()
            });

            productMaingear.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear.Name)),
                DisplayOrder = 0,
            });
            productMaingear.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear.Name)),
                DisplayOrder = 1,
            });
            productMaingear.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productMaingear);

            var productMaingear_associated_1 = new Product
            {
                Name = "MAINGEAR SHIFT Super Stock Gaming Desktop - Red",
                ParentGroupedProductId = productMaingear.Id,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                OldPrice = 4999M,
                Price = 4200M,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            productMaingear_associated_1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear_associated_1.Name)),
                DisplayOrder = 2,
            });
            allProducts.Add(productMaingear_associated_1);
            _productRepository.Insert(productMaingear_associated_1);

            var productMaingear_associated_2 = new Product
            {
                Name = "MAINGEAR SHIFT Super Stock Gaming Desktop - White",
                ParentGroupedProductId = productMaingear.Id,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                OldPrice = 4999M,
                Price = 4200M,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            productMaingear_associated_2.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-White.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear_associated_2.Name)),
                DisplayOrder = 2,
            });
            allProducts.Add(productMaingear_associated_2);
            _productRepository.Insert(productMaingear_associated_2);

            var productMaingear_associated_3 = new Product
            {
                Name = "MAINGEAR SHIFT Super Stock Gaming Desktop - Blue",
                ParentGroupedProductId = productMaingear.Id,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = false, //hide this products
                OldPrice = 4999M,
                Price = 4200M,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            productMaingear_associated_3.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Maingear-Blue.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMaingear_associated_3.Name)),
                DisplayOrder = 2,
            });
            allProducts.Add(productMaingear_associated_3);
            _productRepository.Insert(productMaingear_associated_3);


            //Asus X401
            var productAsusX401 = new Product
            {
                Name = "Asus X401A Laptop",
                OldPrice = 333M,
                Price = 293M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "In Pink with 4GB RAM!",
                FullDescription = "<div><p><strong>The compact 14.1 inch Asus X401 laptop crams plenty of power into an impressively slim body.</strong></p><p>A speed 4Gb RAM and Intel® Celeron® processor can handle all your multitasking jobs quickly and easily, while the 320Gb hard drive gives you storage space for up to 91,000 digital photos, 80,000 MP3s, 140 hours of DVD quality video or 38 hours of HD video.</p><p>A 14.1 inch LED-backlit HD display ensures the Asus X401 laptop makes all your viewing a pleasure, from surfing your favourite websites to viewing your digital photos.</p><p>Use the integrated webcam and mic to enjoy face-to-face chats with friends or family around the world via Skype.</p><p>With HDMI support, the expansion possibilities are limitless, and this versatile Asus X401 14.1 inch laptop can be easily connected to HDMI capable TVs, consoles and entertainment systems</p><ul><li><span>Screen size</span> - <span>14.1in - 1366 x 768</span></li><li><span>Processor</span> - <span>Intel Celeron B815 - 1.6</span></li><li><span>RAM</span> - <span>4GB</span></li><li><span>Hard Drive</span> - <span>320</span></li><li><span>Operating System</span> - <span>Windows 7 Home Premium (64-bit)</span></li><li><span>Optical Drive</span> - <span>NO OD</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul></div>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAsusX401);
            productAsusX401.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAsusX401.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Asus")
            });

            productAsusX401.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Pink")
            });
            productAsusX401.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "4 GB").FirstOrDefault()
            });
            productAsusX401.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "320 GB").FirstOrDefault()
            });

            productAsusX401.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusX401-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusX401.Name)),
                DisplayOrder = 0,
            });
            productAsusX401.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusX401-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusX401.Name)),
                DisplayOrder = 1,
            });
            productAsusX401.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AsusX401-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAsusX401.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productAsusX401);


            //Acer Aspire E1
            var productAcerAspireE1 = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Acer Aspire E1 Core i5",
                OldPrice = 459M,
                Price = 359M,
                ShortDescription = "With the Aspire E1 laptop, everyday activities like web browsing, communicating and playing videos are effortless with powerful Intel Core i5 3rd Gen processing. Advanced Intel HD 4000 graphics deliver fine visual detail, plenty of memory and a brand new Windows 8 operating system means you can launch applications quickly and smoothly, and a large 500GB hard drive provides more space for your media.",
                FullDescription = "<ul><li><span>Screen size</span> - <span>15.6in - 1366 x 768</span></li><li><span>Processor</span> - <span>Intel Core i5 3210M - 2.8</span></li><li><span>RAM</span> - <span>4GB</span></li><li><span>Hard Drive</span> - <span>500</span></li><li><span>Operating System</span> - <span>Windows 8 Home Premium (64-bit)</span></li><li><span>Optical Drive</span> - <span>DVD RW</span></li><li><span>Graphics</span> - <span>Intel HD Graphics 4000</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAcerAspireE1);
            productAcerAspireE1.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAcerAspireE1.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Acer")
            });

            productAcerAspireE1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productAcerAspireE1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "4 GB").FirstOrDefault()
            });
            productAcerAspireE1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "500 GB").FirstOrDefault()
            });

            productAcerAspireE1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerE1-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireE1.Name)),
                DisplayOrder = 0,
            });
            productAcerAspireE1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerE1-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireE1.Name)),
                DisplayOrder = 1,
            });
            productAcerAspireE1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerE1-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireE1.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productAcerAspireE1);


            //Sony VAIO E15 Laptop
            var productSonyVaio = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Sony VAIO E15 Laptop",
                Price = 375M,
                ShortDescription = "Feel the thrill of your games, movies and music, thanks to the great graphics and big, clear ‘xLOUD’ and Dolby Home Theater® sound.",
                FullDescription = "Enjoy quality digital face-to-face time with the HD web camera.<br/>Unleash your inner artist to create cinematic movies and original music soundtracks with the pre-installed software. <br/><ul><li><span>Screen size</span> - <span>15.5in - 1366 x 768</span></li><li><span>Processor</span> - <span>Intel Pentium 2370M - 2.4</span></li><li><span>RAM</span> - <span>4GB</span></li><li><span>Hard Drive</span> - <span>500</span></li><li><span>Operating System</span> - <span>Windows 8</span></li><li><span>Optical Drive</span> - <span>DVD RW</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productSonyVaio);
            productSonyVaio.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });
            productSonyVaio.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });

            //manufacturers
            productSonyVaio.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Sony")
            });

            productSonyVaio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "White/Black")
            });
            productSonyVaio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "4 GB").FirstOrDefault()
            });
            productSonyVaio.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "500 GB").FirstOrDefault()
            });

            productSonyVaio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SonyVaio-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSonyVaio.Name)),
                DisplayOrder = 0,
            });
            productSonyVaio.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SonyVaio-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSonyVaio.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productSonyVaio);


            //Acer Aspire S3-391 Ultrabook
            var productAcerAspireUltrabook = new Product
            {
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                Name = "Acer Aspire S3-391 Ultrabook",
                OldPrice = 642M,
                Price = 599M,
                ShortDescription = "13.3 inch Core i3 Windows 8 Ultrabook in Champagne Gold",
                FullDescription = "<p>Save time with Acer Green Instant On. Put the S3 to sleep and spring it back to life in less than 2 seconds!</p><p>At its thinnest point the Aspire S3 is just 13 mm (0.51\") slim. - The sleek and stylish aluminium and magnesium alloy chassis is strong and light, helping the Aspire S3 weigh less than 1.4 kg</p><p>Thanks to Acer Instant Connect you can get on the Internet from sleep 4x faster&nbsp;than on normal notebooks</p><p>Slender but bold, the Aspire S3 sports professionally tuned speakers with Dolby® Home Theater® v4 for cinema-quality sound - HDMI® connectivity allows you to easily connect your S3 to any compatible TV or monitor</p><p>The Aspire S3 features 2 x super-speed USB 3.0 ports - up to 10x faster than USB 2.0!</p><p>The Aspire S3 features an innovative thermal design that keeps you comfortable even when you use the notebook for hours at a time</p><p>Powered by Intel′s Core i3 processor and a whopping 4GB DDR3 RAM to keep you connected and flying through your day-to-day tasks.</p><ul><li><span>Screen size</span> - <span>13.3in - 1366 x 768</span></li><li><span>Processor</span> - <span>Intel Core i3 2377M - 1.5</span></li><li><span>RAM</span> - <span>4GB</span></li><li><span>Hard Drive</span> - <span>500</span></li><li><span>Optical Drive</span> - <span>NO OD</span></li><li><span>Graphics</span> - <span>Intel</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAcerAspireUltrabook);
            productAcerAspireUltrabook.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAcerAspireUltrabook.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Acer")
            });

            productAcerAspireUltrabook.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Champagne")
            });
            productAcerAspireUltrabook.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "4 GB").FirstOrDefault()
            });
            productAcerAspireUltrabook.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "500 GB").FirstOrDefault()
            });

            var pvaAcerAspireUltrabook = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.Where(x => x.Name == "OS").Single(),
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true,
            };
            pvaAcerAspireUltrabook.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Professional",
                PriceAdjustment = 180,
                DisplayOrder = 1,
            });
            pvaAcerAspireUltrabook.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Windows 7 Ultimate",
                PriceAdjustment = 240,
                DisplayOrder = 2,
            });
            productAcerAspireUltrabook.ProductAttributeMappings.Add(pvaAcerAspireUltrabook);

            productAcerAspireUltrabook.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-acerUltrabook-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireUltrabook.Name)),
                DisplayOrder = 0,
            });
            productAcerAspireUltrabook.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-acerUltrabook-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireUltrabook.Name)),
                DisplayOrder = 1,
            });
            productAcerAspireUltrabook.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-acerUltrabook-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerAspireUltrabook.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productAcerAspireUltrabook);


            //Lenovo IdeaPad Z580
            var productLenovoIdeaPadZ580 = new Product
            {
                Name = "Lenovo IdeaPad Z580",
                Price = 488M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Get Mobile Performance With Some Seriously Impressive Flair!",
                FullDescription = "<p>As a frequent traveler you want to be able to easily take entertainment with you wherever you go, the <strong>Windows 8 </strong>Z580 laptop offers you a great balance of multimedia performance, affordability and bold style so you can be sure to make the best impressions as well as being entertained yourself.<br><br>With the Lenovo IdeaPad Z580, you can be sure that you′re getting&nbsp;the very latest technology that computing has to offer. A&nbsp;<strong>3rd&nbsp;Generation&nbsp;Intel Core i5 </strong>processor is at the heart of this laptop supported&nbsp;by an immense&nbsp;<strong>8GB of DDR3 RAM</strong>, combine that with a colossal <strong>1TB hard drive, </strong>a seriously impressive <strong>Dolby Home Theatre V4</strong> sound system&nbsp;and the brand spanking new <strong>Windows 8 operating system </strong>- you have yourself a multimedia laptop that delivers on all fronts.<br><br>So whether you′re looking for an entertainment and social networking companion or shopping for a laptop with the best multimedia features on the market right now -&nbsp;look no further, you have found it with the affordable Lenovo IdeaPad Z580.</p><ul><li><span>Screen size</span> - <span>15.6in - 1366 x 768</span></li><li><span>Processor</span> - <span>Intel Core i5 3210M - 2.5</span></li><li><span>RAM</span> - <span>8GB</span></li><li><span>Hard Drive</span> - <span>1</span></li><li><span>Operating System</span> - <span>Windows 8 Home Premium (64-bit)</span></li><li><span>Optical Drive</span> - <span>DVD SM</span></li><li><span>Graphics</span> - <span>NVIDIA GeForce GT 630M</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productLenovoIdeaPadZ580);
            productLenovoIdeaPadZ580.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });
            productLenovoIdeaPadZ580.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });

            //manufacturers
            productLenovoIdeaPadZ580.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Lenovo")
            });

            productLenovoIdeaPadZ580.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Red")
            });
            productLenovoIdeaPadZ580.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "8 GB").FirstOrDefault()
            });
            productLenovoIdeaPadZ580.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "1 TB").FirstOrDefault()
            });

            productLenovoIdeaPadZ580.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaPad-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaPadZ580.Name)),
                DisplayOrder = 0,
            });
            productLenovoIdeaPadZ580.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaPad-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaPadZ580.Name)),
                DisplayOrder = 1,
            });
            productLenovoIdeaPadZ580.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaPad-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaPadZ580.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productLenovoIdeaPadZ580);


            //Samsung 350V5C
            var productSamsung350V5C = new Product
            {
                Name = "Samsung 350V5C",
                Price = 349M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Core i3 Windows 8 Laptop in Pink With 6GB RAM!",
                FullDescription = "<ul><li><span>Screen size</span> - <span>15.6 in</span></li><li><span>Processor</span> - <span>Intel Core i3 3110M - 2.4</span></li><li><span>RAM</span> - <span>6GB</span></li><li><span>Hard Drive</span> - <span>500</span></li><li><span>Operating System</span> - <span>Windows 8 Home (64-bit)</span></li><li><span>Optical Drive</span> - <span>DVD SM</span></li><li><span>Graphics</span> - <span>Intel HD Graphics 3000</span></li><li><span>Warranty</span> - <span>1 year warranty</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productSamsung350V5C);
            productSamsung350V5C.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });
            productSamsung350V5C.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Gaming"),
                DisplayOrder = 1,
            });

            //manufacturers
            productSamsung350V5C.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Samsung")
            });

            productSamsung350V5C.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Pink")
            });
            productSamsung350V5C.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "RAM").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "6 GB").FirstOrDefault()
            });
            productSamsung350V5C.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.Where(sa => sa.Name == "HDD").FirstOrDefault()
                .SpecificationAttributeOptions.Where(sao => sao.Name == "500 GB").FirstOrDefault()
            });

            productSamsung350V5C.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungV350C5-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsung350V5C.Name)),
                DisplayOrder = 0,
            });
            productSamsung350V5C.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungV350C5-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsung350V5C.Name)),
                DisplayOrder = 1,
            });
            productSamsung350V5C.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungV350C5-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsung350V5C.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productSamsung350V5C);


            //Acer Iconia B1-A71
            var productAcerIconiaB1A71 = new Product
            {
                Name = "Acer Iconia B1-A71",
                OldPrice = 129M,
                Price = 109M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "8GB 7 inch Android 4.1 Jelly Bean Tablet in Black",
                FullDescription = "<ul><li><span>Screen</span> - <span>7 in Capacitive 1024 x 600 pixels</span></li><li><span>Operating System</span> - <span>Android 4.1</span></li><li><span>Processor</span> - <span>Arm 1.2 GHz</span></li><li><span>Memory</span> - <span>512 MB</span></li><li><span>Storage</span> - <span>8 GB</span></li><li><span>Battery Life</span> - <span>5 hour(s)</span></li><li><span>Wifi</span> - <span>802.11b/g/n</span></li><li><span>3G</span> - <span>No</span></li><li><span>Bluetooth</span> - <span>4.0</span></li><li><span>Camera</span> - <span>0.3MP Front</span></li><li><span>HDMI Ports</span> - <span>None</span></li><li><span>Mini HDMI Ports</span> - <span>None</span></li><li><span>Micro HDMI Ports</span> - <span>None</span></li><li><span>USB 2.0 Ports</span> - <span>None</span></li><li><span>Mini USB 2.0 Ports</span> - <span>None</span></li><li><span>Micro USB 2.0 Ports</span> - <span>1</span></li><li><span>Dimensions (H x W xD)</span> - <span>11.3 mm x 197.4 mm x 128.5 mm</span></li><li><span>Weight</span> - <span>320 g</span></li><li><span>Colour</span> - <span>Black</span></li><li><span>Warranty</span> - <span>1 Year</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productAcerIconiaB1A71);
            productAcerIconiaB1A71.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });

            //manufacturers
            productAcerIconiaB1A71.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Acer")
            });

            productAcerIconiaB1A71.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productAcerIconiaB1A71.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "512 MB")
            });
            productAcerIconiaB1A71.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "8 GB")
            });

            productAcerIconiaB1A71.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerIconia-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 0,
            });
            productAcerIconiaB1A71.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerIconia-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 1,
            });
            productAcerIconiaB1A71.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-AcerIconia-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productAcerIconiaB1A71);


            //Sony Tablet S
            var productSonyTabletS = new Product
            {
                Name = "Sony Tablet S",
                Price = 89M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The unique Tablet S from Sony is ergonomically designed to fit in your hand and shifts weight closer to your palm making your whole multimedia tablet experience feel lighter and overall more comfortable. Its also built in a way that provides a natural angle for typing and browsing while sitting down and relaxing.",
                FullDescription = "<p>Control your TV, Blu-ray Disc™ player, HiFi system, even your cable box from your Sony Tablet S. <br><br>The built-in infra-red Universal Remote is compatible with any brand, so you can <strong>flip channels or adjust the volume of your entire entertainment system</strong> from one screen.</p> <br/><ul><li><span>Screen size</span> - <span>9.4 in - 1280 x 800 pixels</span></li><li><span>Processor</span> - <span>NVIDIA Tegra 2.0 250S - 1 GHz</span></li><li><span>RAM</span> - <span>1 GB</span></li><li><span>Hard Drive</span> - <span>32 GB SSD</span></li><li><span>Operating System</span> - <span>Android 3.1</span></li><li><span>Webcam</span> - <span>5MP Rear / 0.3MP Front</span></li><li><span>Warranty</span> - <span>1 Year</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productSonyTabletS);
            productSonyTabletS.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });

            //manufacturers
            productSonyTabletS.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Sony")
            });

            productSonyTabletS.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productSonyTabletS.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "1 GB")
            });
            productSonyTabletS.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "32 GB")
            });

            productSonyTabletS.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SonyTablet-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 0,
            });
            productSonyTabletS.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SonyTablet-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 1,
            });
            productSonyTabletS.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SonyTablet-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productAcerIconiaB1A71.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productSonyTabletS);


            //Archos 101 XS
            var productArchos101Xs = new Product
            {
                Name = "Archos 101 XS Gen 10",
                OldPrice = 400M,
                Price = 259M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Android 4.0 Ice Cream Sandwich Tablet with Coverboard Keyboard.",
                FullDescription = "ARCHOS unveils the 101XS Gen 10, a new, slim Android tablet: offering a combination of super-fast web-browsing and an elegant slim-line design. ARCHOS continues to push the technological boundaries with the new and innovative Coverboard, a unique keyboard conceived for each tablet model. The 101XS G10 comes with an ultra-thin magnetic Coverboard to perfectly cover, protect, dock and type.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productArchos101Xs);
            productArchos101Xs.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });
            productArchos101Xs.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Notebooks"),
                DisplayOrder = 1,
            });

            productArchos101Xs.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productArchos101Xs.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "512 MB")
            });
            productArchos101Xs.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "8 GB")
            });

            productArchos101Xs.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-ArchosTablet-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productArchos101Xs.Name)),
                DisplayOrder = 0,
            });
            productArchos101Xs.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-ArchosTablet-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productArchos101Xs.Name)),
                DisplayOrder = 1,
            });
            productArchos101Xs.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-ArchosTablet-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productArchos101Xs.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productArchos101Xs);


            //Lenovo IdeaTab
            var productLenovoIdeaTab = new Product
            {
                Name = "Lenovo IdeaTab A2109A",
                OldPrice = 350M,
                Price = 199M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Stylish, sleek and built to entertain, the Lenovo IdeaTab A2109 9\" Tablet will provide you with exciting handheld entertainment.",
                FullDescription = "<p><strong>HD entertainment </strong><br>The Lenovo A2109 Tablet delivers a smooth and enjoyable experience with excellent execution. <br>With its 1200 x 800 HD screen, the Lenovo IdeaTab A2109 will provide you with detailed visuals that are sure to entertain when playing games, watching movies or playing with an app. <br>If you want to share your Lenovo's display with the rest of your household then you can do so via the micro HDMI as you hook up to your big screen TV. <br><br><strong>Functional features </strong><br>The Lenovo 9\" Tablet A2109 IdeaTab is operated by Android 4.0 Ice Cream Sandwich, providing you with an intuitive handheld experience that combines with the quad-core Nvidia Tegra 3 processor for excellent multitasking. <br><br>Connect to the internet via the built-in WiFi, enabling you to chat away on social networking, read the news and surf the web on your favourite sites. <br><br>You will be able to customise your tablet exactly as you like with thousands of apps available to download via Google Playstore.<br><br>This tablet has an impressive battery life of up to 8 hours, giving you plenty of scope for browsing the net, viewing films and enjoying your music without the fear of it requiring a constant recharge. <br><br><strong>Superb storage </strong><br>There is 16 GB of memory, providing suitable space for your multimedia, apps and other programs. You can expand the memory and transfer data thanks to the micro SD card slot. <br><br><strong>Sturdy design </strong><br>The Lenovo A2109 IdeaTab 9\" Tablet has a real metal exterior chassis, making it more robust than other tablets. <br><br>The back cover's roll cage design looks the part too, giving the tablet that extra wow factor. <br>For immersive multimedia on the go, the Lenovo IdeaTab A2109 9\" Tablet has the power, style and flexibility you need</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productLenovoIdeaTab);
            productLenovoIdeaTab.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });

            //manufacturers
            productLenovoIdeaTab.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Lenovo")
            });

            productLenovoIdeaTab.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });
            productLenovoIdeaTab.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "RAM")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "1 GB")
            });
            productLenovoIdeaTab.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "HDD")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "16 GB")
            });

            productLenovoIdeaTab.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaTab-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaTab.Name)),
                DisplayOrder = 0,
            });
            productLenovoIdeaTab.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaTab-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaTab.Name)),
                DisplayOrder = 1,
            });
            productLenovoIdeaTab.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-LenovoIdeaTab-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productLenovoIdeaTab.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productLenovoIdeaTab);


            //55" LED Display
            var product55LedDisplay = new Product
            {
                Name = "55\" LED Display 1920x1080",
                Price = 2599M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "D-Sub DVI HDMI - Black Built-in Speakers",
                FullDescription = "<ul><li><span>Screen size</span> - <span>55</span></li><li><span>Display Format</span> - <span>720p</span></li><li><span>Image Contrast  Ratio</span> - <span></span></li><li><span>Brightness</span> - <span>600 cd/m2</span></li><li><span>Colour</span> - <span>Black</span></li><li><span>Dimensions (WxDxH)</span> - <span>1246.4 mm x 29.9 mm x 718.2 mm</span></li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = true,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(product55LedDisplay);
            product55LedDisplay.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Monitors"),
                DisplayOrder = 1,
            });

            product55LedDisplay.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            product55LedDisplay.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-55LedDisplay-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(product55LedDisplay.Name)),
                DisplayOrder = 0,
            });

            _productRepository.Insert(product55LedDisplay);


            //Dell U2713HM
            var productDellU2713Hm = new Product
            {
                Name = "Dell U2713HM 27\"",
                Price = 499M,
                ShortDescription = "Wide LED Black Monitor 2560x1440 8ms DVI VGA DP HDMI USB",
                FullDescription = "Cinema-like clarity shines on the expansive 27\" Dell UltraSharp U2713HM monitor. Comfort settings help keep you productive. Work comfortably and view images from a variety of positions with swivel and tilt. Easily switch to portrait mode with pivot. Set your Dell UltraSharp U2713HM monitor to the optimum height with a height-adjustability range of 115 mm",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productDellU2713Hm);
            productDellU2713Hm.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Monitors"),
                DisplayOrder = 1,
            });

            //manufacturers
            productDellU2713Hm.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Dell")
            });

            productDellU2713Hm.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Gray")
            });

            productDellU2713Hm.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-DellMonitor-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productDellU2713Hm.Name)),
                DisplayOrder = 0,
            });
            productDellU2713Hm.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-DellMonitor-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productDellU2713Hm.Name)),
                DisplayOrder = 1,
            });
            productDellU2713Hm.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-DellMonitor-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productDellU2713Hm.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productDellU2713Hm);


            //Samsung SyncMaster
            var productSamsungSyncMaster = new Product
            {
                Name = "Samsung SyncMaster S27A850D",
                Price = 549M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "68.6 cm 27inch LED LCD Monitor",
                FullDescription = "Reinvent your desk with the Samsung SA850 LED Monitor. It brings together the very best monitor features like the outstanding LED clarity and WQHD screen resolution, to deliver a totally professional experience. It offers the support needed for all your content, like your videos and pictures, while also providing optimised viewing with the convenient Wide Angle Viewing feature. So, why not make the move to the LED monitor that is raising the standards and expectations for everyone.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productSamsungSyncMaster);
            productSamsungSyncMaster.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Monitors"),
                DisplayOrder = 1,
            });

            //manufacturers
            productSamsungSyncMaster.ProductManufacturers.Add(new ProductManufacturer
            {
                DisplayOrder = 0,
                Manufacturer = _manufacturerRepository.Table.FirstOrDefault(c => c.Name == "Samsung")
            });

            productSamsungSyncMaster.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            productSamsungSyncMaster.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungSyncMaster-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsungSyncMaster.Name)),
                DisplayOrder = 0,
            });
            productSamsungSyncMaster.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungSyncMaster-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsungSyncMaster.Name)),
                DisplayOrder = 1,
            });
            productSamsungSyncMaster.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-SamsungSyncMaster-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productSamsungSyncMaster.Name)),
                DisplayOrder = 2,
            });

            _productRepository.Insert(productSamsungSyncMaster);


            //Iiyama T1531SAWB1
            var productIiyamaT1531Sawb1 = new Product
            {
                Name = "Iiyama T1531SAWB1",
                OldPrice = 350M,
                Price = 299M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "15\" LCD Touch Screen Monitor Surface Acoustic Wave 1024x768",
                FullDescription = "The ProLite T1531SAW-1 uses highly accurate Surface Acoustic Wave Touch Technology (SAW) with a pure-glass scratch resistive front, offering superior image clarity and screen durability. The screen can be touched with a soft stylus, finger or gloved hand making it suitable for a number of applications, and the robust design and solid but flexible stand ensures resilience in the most demanding environment. Menu Buttons are located on the side of the screen which can be locked to prevent tampering, and include a handy function to deactivate the Touch Screen for cleaning . The iiyama Prolite T1531SAW-1 Touch Screen is ideal for a wide range of applications including Point of Sale, Kiosks, Hospitality and Entertainment, Control rooms, Education,",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productIiyamaT1531Sawb1);
            productIiyamaT1531Sawb1.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Monitors"),
                DisplayOrder = 1,
            });

            productIiyamaT1531Sawb1.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            productIiyamaT1531Sawb1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Liyama-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productIiyamaT1531Sawb1.Name)),
                DisplayOrder = 0,
            });
            productIiyamaT1531Sawb1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Liyama-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productIiyamaT1531Sawb1.Name)),
                DisplayOrder = 1,
            });
            productIiyamaT1531Sawb1.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Liyama-3.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productIiyamaT1531Sawb1.Name)),
                DisplayOrder = 2,
            });
            _productRepository.Insert(productIiyamaT1531Sawb1);


            //The Original Writing Tablet in Red
            var productWritingTablet = new Product
            {
                Name = "The Original Writing Tablet in Red",
                Price = 29M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The original Boogie Board LCD eWriter that started a revolution.",
                FullDescription = "Ultra light at just 4oz and 3mm thin. One just isn't enough. Get one for on the go, in the office and multiple places around the house. Almost anywhere you'd keep memo pads, sticky notes or scrap paper.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productWritingTablet);
            productWritingTablet.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });
            productWritingTablet.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });

            productWritingTablet.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Red")
            });

            productWritingTablet.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-writingTablet-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productWritingTablet.Name)),
                DisplayOrder = 0,
            });
            productWritingTablet.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-writingTablet-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productWritingTablet.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productWritingTablet);


            //Cyborg White R.A.T 7 Contagion Mouse
            var productMouse = new Product
            {
                Name = "Cyborg White R.A.T 7 Contagion Mouse",
                Price = 29M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Still uncompromising, unparalleled and unmatched.",
                FullDescription = "Looking ultra fresh in white and silver, with a next generation 6400 DPI sensor that further improves on the incredible tracking and precision of the original, the R.A.T.7 Contagion is now even more eye-catching than before.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productMouse);
            productMouse.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });
            productMouse.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });

            productMouse.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "White")
            });

            productMouse.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Mouse-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMouse.Name)),
                DisplayOrder = 0,
            });
            productMouse.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-Mouse-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productMouse.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productMouse);


            //Classic Line G83-6105 USB Keyboard
            var productKeyboard = new Product
            {
                Name = "Classic Line G83-6105 USB Keyboard",
                Price = 20M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "This keyboard has been one of the most reliable and successful on the market for many years, with more than 30 million satisfied users.",
                FullDescription = "<ul><li><strong>Interface:</strong> USB</li><li><strong>Colour:</strong> Black</li><li><strong>Weight:</strong> 662g </li><li><strong>Cable Length:</strong> approx. 1.75 m </li><li><strong>Dimensions</strong>: approx. 458 x 170 x 42 mm </li><li><strong>Warranty:</strong> 2 years</li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productKeyboard);
            var pvaKeyboard = new ProductAttributeMapping()
            {
                ProductAttribute = _productAttributeRepository.Table.FirstOrDefault(x => x.Name == "Color"),
                AttributeControlType = AttributeControlType.ColorSquares,
                IsRequired = true,
            };
            pvaKeyboard.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "White",
                IsPreSelected = true,
                ColorSquaresRgb = "#ffffff",
                DisplayOrder = 1,
            });
            pvaKeyboard.ProductAttributeValues.Add(new ProductAttributeValue()
            {
                AttributeValueType = AttributeValueType.Simple,
                Name = "Black",
                ColorSquaresRgb = "#000000",
                DisplayOrder = 2,
            });
            productKeyboard.ProductAttributeMappings.Add(pvaKeyboard);

            productKeyboard.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });
            productKeyboard.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });

            productKeyboard.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            productKeyboard.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "White")
            });

            productKeyboard.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-keyboard-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productKeyboard.Name)),
                DisplayOrder = 0,
            });

            _productRepository.Insert(productKeyboard);


            //Netbooks Charging Trolley
            var productNetbooksCharging = new Product
            {
                Name = "Netbooks Charging Trolley",
                OldPrice = 1500M,
                Price = 1299M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "The UnoCart™ NET16 from LapSafe® Products is designed to far exceed the abilities of its rivals at a competitive price, incorporating: simultaneous charging, safe power management, twin thermostatically controlled fans and effective cable management.",
                FullDescription = "<p><strong>DIMENSIONS</strong> <br>Width 62.5cm <br>Height 84.0cm <br>Depth 55.0cm</p>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productNetbooksCharging);
            productNetbooksCharging.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });
            productNetbooksCharging.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Tablets"),
                DisplayOrder = 1,
            });

            productNetbooksCharging.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Red")
            });

            productNetbooksCharging.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-netbooksCharging-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productNetbooksCharging.Name)),
                DisplayOrder = 0,
            });

            _productRepository.Insert(productNetbooksCharging);


            //TP-Link Wi-Fi N300 ADSL2 
            var productTpLinkN = new Product
            {
                Name = "TP-Link Wi-Fi N300 ADSL2",
                Price = 29M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "300Mbps Wireless N ADSL2+ Modem Router - TD-W8960N.",
                FullDescription = "<ul><li><strong>Interface</strong>: <br>4 10/100Mbps RJ45 Ports <br>1 RJ11 Port </li><li><strong>Button</strong>:<br>1 Power On/Off Switch <br>1 QSS Button </li><li><strong>External Power Supply</strong>: 12VDC/1A </li><li><strong>IEEE Standards</strong>: IEEE 802.3, 802.3u </li><li><strong>ADSL Standards</strong>: &nbsp;Full-rate ANSI T1.413 Issue 2, ITU-T G.992.1(G.DMT), ITU-T G.992.2(G.Lite) ITU-T G.994.1 (G.hs), ITU-T G.995.1 </li><li><strong>ADSL2 Standards</strong>: ITU-T G.992.3 (G.dmt.bis), ITU-T G.992.4 (G.lite.bis) </li><li><strong>ADSL2+ Standards</strong>: ITU-T G.992.5 </li><li><strong>Dimensions </strong>( W x D x H ): 7.9*5.5*1.1 in.(200*140*28 mm) </li><li><strong>Antenna Type</strong>: Omni directional, Detachable, Reverse SMA </li><li><strong>Antenna Gain</strong>: 2x3dBi </li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productTpLinkN);
            productTpLinkN.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });

            productTpLinkN.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Blue")
            });

            productTpLinkN.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-TpLinkN-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productTpLinkN.Name)),
                DisplayOrder = 0,
            });

            _productRepository.Insert(productTpLinkN);


            //TP-Link TL-MR3020 Portable Wireless N Router
            var productTpLinkMr = new Product
            {
                Name = "TP-Link TL-MR3020 Portable N Router",
                Price = 31M,
                ShortDescription = "Share the freedom of 3G!",
                FullDescription = "Powered by a laptop or power adapter, the TL-MR3020 allows user to easily share a 3G/3.75G mobile connection with family and friends on the train, while camping, at the hotel, nearly anywhere within 3G/ 3.75G coverage. By connecting a 3G USB modem to the router, a 3G Wi-Fi hotspot is instantly established.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productTpLinkMr);
            productTpLinkMr.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });

            productTpLinkMr.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Gray")
            });

            productTpLinkMr.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-TpLinkTl-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productTpLinkMr.Name)),
                DisplayOrder = 0,
            });
            productTpLinkMr.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-TpLinkTl-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productTpLinkMr.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productTpLinkMr);

            //TP-Link 300Mbps AV200 Wireless N Powerline Adapter
            var productTpLinkAv = new Product
            {
                Name = "TP-Link 300Mbps AV200 Wireless N Adapter",
                OldPrice = 99M,
                Price = 64M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Extend the reach of your Internet throughout all of your home with the TP-Link 200M Powerline Adapter with Wi-Fi N300 Access Point.",
                FullDescription = "The TP-LINK Wireless N Powerline Extender, TL-WPA281kit extends an Internet connection to every room connected to a home’s circuitry. All devices in the room have two options to access the TL-WPA281kit, through wireless or an Ethernet cable. With 200Mbps Powerline link rate and 300Mbps wireless N data transfer rate, it is ideal for bandwidth consuming or latency sensitive applications like video streaming, online gaming and Internet calls. With TL-WPA281kit, Internet can now truly be available everywhere in your home or offce.",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productTpLinkAv);
            productTpLinkAv.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });
            productTpLinkAv.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });

            productTpLinkAv.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "White/Black")
            });

            productTpLinkAv.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-TpLink-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productTpLinkAv.Name)),
                DisplayOrder = 0,
            });
            productTpLinkAv.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-TpLink-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productTpLinkAv.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productTpLinkAv);



            //Belkin Smart TV Link 4 Port 
            var productBelkinSmart = new Product
            {
                Name = "Belkin Smart TV Link 4 Port ",
                Price = 99M,
                ProductType = ProductType.SimpleProduct,
                VisibleIndividually = true,
                ShortDescription = "Wirelessly connect your Smart TV and up to 3 other devices to the Internet. Bring Wi-Fi® to the living room, and leave the router in your home office.",
                FullDescription = "<ul><li>Dual-Band Performance Simultaneous 2.4GHz and 5GHz streaming </li><li>deal for media and video games</li><li>Easy Setup Push-button WPS or simple setup wizard </li></ul>",
                ProductTemplateId = productTemplateSimple.Id,
                //SeName = "",
                AllowCustomerReviews = true,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomePage = false,
                IsShipEnabled = true,
                Weight = 6,
                Length = 19,
                Width = 10,
                Height = 10,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
            };
            allProducts.Add(productBelkinSmart);
            productBelkinSmart.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Network"),
                DisplayOrder = 1,
            });
            productBelkinSmart.ProductCategories.Add(new ProductCategory
            {
                Category =
                    _categoryRepository.Table.FirstOrDefault(c => c.Name == "Accessories"),
                DisplayOrder = 1,
            });

            productBelkinSmart.ProductSpecificationAttributes.Add(new ProductSpecificationAttribute()
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOption = _specificationAttributeRepository.Table.FirstOrDefault(sa => sa.Name == "Color")
                                                .SpecificationAttributeOptions.FirstOrDefault(sao => sao.Name == "Black")
            });

            productBelkinSmart.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-BelkinSmart-1.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productBelkinSmart.Name)),
                DisplayOrder = 0,
            });
            productBelkinSmart.ProductPictures.Add(new ProductPicture
            {
                Picture =
                    _pictureService.InsertPicture(
                        File.ReadAllBytes(sampleImagesPath +
                                          "product-BelkinSmart-2.jpg"), "image/jpeg",
                        _pictureService.GetPictureSeName(productBelkinSmart.Name)),
                DisplayOrder = 1,
            });

            _productRepository.Insert(productBelkinSmart);


            //search engine names
            foreach (var product in allProducts)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = product.Id,
                    EntityName = "Product",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = product.ValidateSeName("", product.Name, true)
                });
            }


            //related products
            var relatedProducts = new List<RelatedProduct>
                                      {
                                          new RelatedProduct
                                              {
                                                  ProductId1 = product55LedDisplay.Id,
                                                  ProductId2 = productAcerAspireE1.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = product55LedDisplay.Id,
                                                  ProductId2 = productSamsung350V5C.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = product55LedDisplay.Id,
                                                  ProductId2 = productSamsungSyncMaster.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireE1.Id,
                                                  ProductId2 = productArchos101Xs.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireE1.Id,
                                                  ProductId2 = productLenovoIdeaTab.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireE1.Id,
                                                  ProductId2 = product55LedDisplay.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireUltrabook.Id,
                                                  ProductId2 = productAcerIconiaB1A71.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireUltrabook.Id,
                                                  ProductId2 = productAcerPredatorG3.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAcerAspireUltrabook.Id,
                                                  ProductId2 = productAceracerAz500.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAsusEssentio.Id,
                                                  ProductId2 = productAsusX401.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAsusEssentio.Id,
                                                  ProductId2 = productBelkinSmart.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAsusEssentio.Id,
                                                  ProductId2 = productCyberpower.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productDellU2713Hm.Id,
                                                  ProductId2 = productAsusX401.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productDellU2713Hm.Id,
                                                  ProductId2 = productHpPavilionp7.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productDellU2713Hm.Id,
                                                  ProductId2 = productHpTouchSmart.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productKeyboard.Id,
                                                  ProductId2 = productLenovoIdeaPadZ580.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productKeyboard.Id,
                                                  ProductId2 = productLenovoIdeaTab.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productKeyboard.Id,
                                                  ProductId2 = productLenovoK430.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productMouse.Id,
                                                  ProductId2 = productNetbooksCharging.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productMouse.Id,
                                                  ProductId2 = productSamsung350V5C.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productMouse.Id,
                                                  ProductId2 = productSamsungSyncMaster.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productSonyVaio.Id,
                                                  ProductId2 = product55LedDisplay.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productSonyVaio.Id,
                                                  ProductId2 = productAcerAspireE1.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productSonyVaio.Id,
                                                  ProductId2 = productAcerAspireUltrabook.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productAceracerAz500.Id,
                                                  ProductId2 = productArchos101Xs.Id,
                                              },
                                               new RelatedProduct
                                              {
                                                  ProductId1 = productAceracerAz500.Id,
                                                  ProductId2 = productBelkinSmart.Id,
                                              },
                                               new RelatedProduct
                                              {
                                                  ProductId1 = productAceracerAz500.Id,
                                                  ProductId2 = productHpPavilionp7.Id,
                                              },

                                              new RelatedProduct
                                              {
                                                  ProductId1 = productHpPavilionp7.Id,
                                                  ProductId2 = productCyberpower.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productHpPavilionp7.Id,
                                                  ProductId2 = productDellU2713Hm.Id,
                                              },
                                              new RelatedProduct
                                              {
                                                  ProductId1 = productHpPavilionp7.Id,
                                                  ProductId2 = productLenovoIdeaPadZ580.Id,
                                              },
                                      };
            relatedProducts.ForEach(rp => _relatedProductRepository.Insert(rp));

            //product tags
            AddProductTag(product55LedDisplay, "awesome");
            AddProductTag(productAcerAspireE1, "awesome");
            AddProductTag(productAcerAspireUltrabook, "awesome");
            AddProductTag(productArchos101Xs, "awesome");
            AddProductTag(productLenovoIdeaPadZ580, "awesome");
            AddProductTag(productLenovoK430, "awesome");
            AddProductTag(productKeyboard, "awesome");
            AddProductTag(productMaingear, "awesome");
            AddProductTag(productMouse, "awesome");
            AddProductTag(productSonyTabletS, "awesome");
            AddProductTag(productNetbooksCharging, "awesome");

            AddProductTag(productLenovoIdeaTab, "powerful");
            AddProductTag(productHpTouchSmart, "powerful");
            AddProductTag(productDellInspiron560, "powerful");
            AddProductTag(productAsusX401, "powerful");
            AddProductTag(productAsusEssentio, "powerful");
            AddProductTag(productToshibaQosmio, "powerful");
            AddProductTag(productSonyVaio, "powerful");
            AddProductTag(productSamsung350V5C, "powerful");

            AddProductTag(productLenovoIdeaTab, "stylish");
            AddProductTag(productAsusX401, "stylish");
            AddProductTag(productDellInspiron560, "stylish");
            AddProductTag(productAcerPredatorG3, "stylish");
            AddProductTag(productSonyVaio, "stylish");
            AddProductTag(productIiyamaT1531Sawb1, "stylish");

            AddProductTag(productKeyboard, "home");
            AddProductTag(productLenovoIdeaPadZ580, "home");
            AddProductTag(productLenovoK430, "home");
            AddProductTag(productSamsung350V5C, "home");
            AddProductTag(productSonyVaio, "home");

            AddProductTag(productSonyVaio, "multimedia");
            AddProductTag(productSonyTabletS, "multimedia");
            AddProductTag(productSamsung350V5C, "multimedia");
            AddProductTag(productMouse, "multimedia");
            AddProductTag(productLenovoK430, "multimedia");

            AddProductTag(productAsusEssentio, "light");
            AddProductTag(productMouse, "light");
            AddProductTag(productHpTouchSmart, "light");


            AddProductTag(productSamsung350V5C, "for girls");
            AddProductTag(productAsusX401, "for girls");

            AddProductTag(productAcerPredatorG3, "gaming");
            AddProductTag(productMouse, "gaming");
            AddProductTag(productHpPavilionp7, "gaming");
            AddProductTag(productKeyboard, "gaming");

            //reviews
            var random = new Random();
            var adminCustomerRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Administrators);
            if (adminCustomerRole == null)
                throw new Exception("Administrators role cannot be loaded");
            var customer = _customerService.GetAllCustomers(
                customerRoleIds: new int[] { adminCustomerRole.Id },
                pageIndex: 0,
                pageSize: 1).FirstOrDefault();

            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);
                product.ProductReviews.Add(new ProductReview()
                {
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = string.Format("This sample review is for the {0}. I've been waiting for this product to be available. It is priced just right.", product.Name),
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });
                product.ApprovedRatingSum = rating;
                product.ApprovedTotalReviews = product.ProductReviews.Count;

                _productRepository.Update(product);
            }
        }

        #endregion

        #region Ctor

        public ComputerPlugin(
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductTemplate> productTemplateRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<Category> categoryRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<Language> languageRepository,
            IRepository<NewsItem> newsItemRepository,
            IRepository<Product> productRepository,

            IGenericAttributeService genericAttributeService,
            ICustomerService customerService,
            IPictureService pictureService,
            ISettingService settingService,
            IWidgetService widgetService,

            LocalizationSettings localizationSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            WidgetSettings widgetSettings,
            MediaSettings mediaSettings,

            IWebHelper webHelper
            )
        {
            this._specificationAttributeRepository = specificationAttributeRepository;
            this._manufacturerTemplateRepository = manufacturerTemplateRepository;
            this._categoryTemplateRepository = categoryTemplateRepository;
            this._productAttributeRepository = productAttributeRepository;
            this._productTemplateRepository = productTemplateRepository;
            this._relatedProductRepository = relatedProductRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._productTagRepository = productTagRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._categoryRepository = categoryRepository;
            this._blogPostRepository = blogPostRepository;
            this._languageRepository = languageRepository;
            this._newsItemRepository = newsItemRepository;
            this._productRepository = productRepository;

            this._genericAttributeService = genericAttributeService;
            this._customerService = customerService;
            this._settingService = settingService;
            this._pictureService = pictureService;
            this._widgetService = widgetService;

            this._localizationSettings = localizationSettings;
            this._catalogSettings = catalogSettings;
            this._commonSettings = commonSettings;
            this._widgetSettings = widgetSettings;
            this._mediaSettings = mediaSettings;

            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ThemeHelperComputer";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ThemeHelper.Computer.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ComputerSettings()
            {
                Slide1Html = "<div><img title='Great computer!' alt='Great computer!' src='/Themes/Computer/Content/images/slide1.jpg' width='939' height='379' /><a class='slider-button' href='/samsung-350v5c'>Shop Now</a></div>",
                Slide2Html = "<div><img title='Great computer!' alt='Great computer!' src='/Themes/Computer/Content/images/slide2.jpg' width='939' height='379' /><a class='slider-button' href='/samsung-syncmaster-s27a850d'>Shop Now</a></div>",
                Slide3Html = "<div><img title='Great computer!' alt='Great computer!' src='/Themes/Computer/Content/images/slide3.jpg' width='939' height='379' /><a class='slider-button' href='/hp-touchsmart-310-1126'>Shop Now</a></div>",
                PromotionInfo = "<div style='background: #c50027;float: left;padding: 5px 45px;color: white;font-size: 20px;margin-right: 15px;margin-bottom: 5px;font-weight: bold;line-height: 1'>FREE SHIPPING</div><div style='padding: 7px 30px;color: white;font-size: 14px'>On orders over $500. This offer is valid an all our computer store items</div>"
            };
            _settingService.SaveSetting(settings);

            //update public locales 
            this.AddOrUpdatePluginLocaleResource("Shoppingcart.Headerquantity", "{0}");

            //new admin locales 
            this.AddOrUpdatePluginLocaleResource("Computer.Configuration", "Configuration");
            this.AddOrUpdatePluginLocaleResource("Computer.PreconfigureSystem", "Preconfigure system");
            this.AddOrUpdatePluginLocaleResource("Computer.Preconfigure", "Preconfigure");
            this.AddOrUpdatePluginLocaleResource("Computer.PreconfigureCompleted", "Preconfigure completed!");
            this.AddOrUpdatePluginLocaleResource("Computer.PreconfigureError", "Preconfigure error: ");

            this.AddOrUpdatePluginLocaleResource("Computer.SampleData", "Sample data");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.InstallSampleData", "Install sample data");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.InstallationCompleted", "Installation completed!");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.InstallationError", "Installation error: ");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.DataAlreadyInstalled", "The data is already installed!");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.Installing", "Wait...");
            this.AddOrUpdatePluginLocaleResource("Computer.SampleData.Note", "Install sample data only on the clean installation! Click only once!");

            this.AddOrUpdatePluginLocaleResource("Computer.Slide1Html", "Slide1 content");
            this.AddOrUpdatePluginLocaleResource("Computer.Slide2Html", "Slide2 content");
            this.AddOrUpdatePluginLocaleResource("Computer.Slide3Html", "Slide3 content");
            this.AddOrUpdatePluginLocaleResource("Computer.Slide1Html.Hint", "Enter the content of the first slide. It'll be displayed on the home page.");
            this.AddOrUpdatePluginLocaleResource("Computer.Slide2Html.Hint", "Enter the content of the second slide. It'll be displayed on the home page.");
            this.AddOrUpdatePluginLocaleResource("Computer.Slide3Html.Hint", "Enter the content of the third slide. It'll be displayed on the home page.");

            this.AddOrUpdatePluginLocaleResource("Computer.PromotionInfo", "Promotion information");
            this.AddOrUpdatePluginLocaleResource("Computer.PromotionInfo.Hint", "Enter the promotion information. It'll be displayed on the home page.");
            this.AddOrUpdatePluginLocaleResource("Computer.ChangesSaved", "Changes have been saved!");

            this.AddOrUpdatePluginLocaleResource("Computer.Category.MenuIcon", "Category icon");
            this.AddOrUpdatePluginLocaleResource("Computer.Category.MenuIcon.Note", "Note: Recommended maximum icon size is 30 pixels");
            this.AddOrUpdatePluginLocaleResource("Computer.Category.MenuIcon.Hint", "Picture which will be displayed near the category in the main menu.");
            this.AddOrUpdatePluginLocaleResource("Computer.Category.MenuIconTab", "Menu category icon");

            //new public locales
            this.AddOrUpdatePluginLocaleResource("Computer.WelcomeVisitor", "Welcome visitor you can ");
            this.AddOrUpdatePluginLocaleResource("Computer.Or", " or ");
            this.AddOrUpdatePluginLocaleResource("Computer.Register", "create an account");
            this.AddOrUpdatePluginLocaleResource("Computer.Login", "login");
            this.AddOrUpdatePluginLocaleResource("Computer.Logout", "Logout");
            this.AddOrUpdatePluginLocaleResource("Computer.YouAreLogged", "You are logged in as ");
            this.AddOrUpdatePluginLocaleResource("Computer.Call.Message", "CALL");
            this.AddOrUpdatePluginLocaleResource("Computer.ShoppingCart.Items", "item(s)");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ComputerSettings>();

            //update public locales 
            this.AddOrUpdatePluginLocaleResource("Shoppingcart.Headerquantity", "({0})");

            //new admin locales 
            this.DeletePluginLocaleResource("Computer.Configuration");
            this.DeletePluginLocaleResource("Computer.PreconfigureSystem");
            this.DeletePluginLocaleResource("Computer.PreConfigure");
            this.DeletePluginLocaleResource("Computer.PreconfigureCompleted");
            this.DeletePluginLocaleResource("Computer.PreconfigureError");

            this.DeletePluginLocaleResource("Computer.SampleData");
            this.DeletePluginLocaleResource("Computer.SampleData.InstallSampleData");
            this.DeletePluginLocaleResource("Computer.SampleData.InstallationCompleted");
            this.DeletePluginLocaleResource("Computer.SampleData.InstallationError");
            this.DeletePluginLocaleResource("Computer.SampleData.DataAlreadyInstalled");
            this.DeletePluginLocaleResource("Computer.SampleData.Installing");
            this.DeletePluginLocaleResource("Computer.SampleData.Note");

            this.DeletePluginLocaleResource("Computer.Slide1Html");
            this.DeletePluginLocaleResource("Computer.Slide2Html");
            this.DeletePluginLocaleResource("Computer.Slide3Html");
            this.DeletePluginLocaleResource("Computer.Slide1Html.Hint");
            this.DeletePluginLocaleResource("Computer.Slide2Html.Hint");
            this.DeletePluginLocaleResource("Computer.Slide3Html.Hint");

            this.DeletePluginLocaleResource("Computer.PromotionInfo");
            this.DeletePluginLocaleResource("Computer.PromotionInfo.Hint");
            this.DeletePluginLocaleResource("Computer.ChangesSaved");

            this.DeletePluginLocaleResource("Computer.Category.MenuIcon");
            this.DeletePluginLocaleResource("Computer.Category.MenuIcon.Note");
            this.DeletePluginLocaleResource("Computer.Category.MenuIcon.Hint");
            this.DeletePluginLocaleResource("Computer.Category.MenuIconTab");

            //new public locales
            this.DeletePluginLocaleResource("Computer.WelcomeVisitor");
            this.DeletePluginLocaleResource("Computer.Or");
            this.DeletePluginLocaleResource("Computer.Register");
            this.DeletePluginLocaleResource("Computer.Login");
            this.DeletePluginLocaleResource("Computer.Logout");
            this.DeletePluginLocaleResource("Computer.YouAreLogged");
            this.DeletePluginLocaleResource("Computer.Call.Message");
            this.DeletePluginLocaleResource("Computer.ShoppingCart.Items");

            base.Uninstall();
        }

        public void Preconfigure()
        {
            //media settings
            _mediaSettings.ManufacturerThumbPictureSize = 140;
            _mediaSettings.ProductThumbPictureSize = 190;
            _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage = 120;
            _mediaSettings.CategoryThumbPictureSize = 155;
            _settingService.SaveSetting(_mediaSettings);

            //language settings
            _localizationSettings.UseImagesForLanguageSelection = true;
            _settingService.SaveSetting(_localizationSettings);

            //catalog settings
            _catalogSettings.ProductsByTagPageSizeOptions = "6, 3, 12, 24";
            _catalogSettings.ManufacturersBlockItemsToDisplay = 15;
            _settingService.SaveSetting(_catalogSettings);

            //common settings
            _commonSettings.BreadcrumbDelimiter = "|";
            _settingService.SaveSetting(_commonSettings);

            //disable Nivo Slider widget
            var nivoSliderWidget = _widgetService.LoadWidgetBySystemName("Widgets.NivoSlider");
            if (nivoSliderWidget != null && nivoSliderWidget.IsWidgetActive(_widgetSettings))
            {
                //mark as disabled
                _widgetSettings.ActiveWidgetSystemNames.Remove(nivoSliderWidget.PluginDescriptor.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }
        }

        public void InstallSampleData()
        {
            InstallBlogPosts();
            InstallNews();
            InstallSpecificationAttributes();
            InstallCategories();
            InstallManufacturers();
            InstallProductAttributes();
            InstallProducts();
        }

        #endregion
    }
}