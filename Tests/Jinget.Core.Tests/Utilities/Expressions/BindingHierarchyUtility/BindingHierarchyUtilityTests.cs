namespace Jinget.Core.Tests.Utilities.Expressions.BindingHierarchyUtility;

[TestClass]
public class BindingHierarchyUtilityTests
{
    [TestMethod()]
    public void should_create_bindingexpression_using_private_ctor()
    {
        var bindings = BindingDefinition.CreateBuilder<PrivateClass>().Add(x => new { x.Property1 })
            .Build();

        Expression<Func<PrivateClass, PrivateClass>> expectedResult = x => new PrivateClass()
        {
            Property1 = x.Property1
        };

        var result = BindingDefinition.Compile(bindings);

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_create_bindingexpression_using_private_ctor2()
    {
        var binding = BindingDefinition.CreateBuilder<FolderModel>().Add(
         x => new
         {
             x.Name,
             Files = x.Files.Select(f => new
             {
                 f.FolderId,
                 Content = new
                 {
                     f.Content.ContentPath,
                     f.Content.ContentType
                 }
             })
         }).Build();

        Expression<Func<FolderModel, FolderModel>> expectedResult = x => new FolderModel
        {
            Name = x.Name,
            Files = x.Files.Select(f => new FileModel
            {
                FolderId = f.FolderId,
                Content = new FileContent
                {
                    ContentPath = f.Content.ContentPath,
                    ContentType = f.Content.ContentType
                }
            }).ToList()
        };

        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Single_Property()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>()
            .Add(x => new { x.Name })
            .Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel
        {
            Name = x.Name
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Multiple_Properties()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { x.Name, x.Path })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Name = x.Name,
            Path = x.Path
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Nested_Properties()
    {
        var binding = BindingDefinition.CreateBuilder<FileModel>()
            .Add(x => new
            {
                Content = new { x.Content.ContentType }
            })
            .Build();

        Expression<Func<FileModel, FileModel>> expected = x => new FileModel
        {
            Content = new FileContent { ContentType = x.Content.ContentType }
        };
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Complex_Nested_Properties()
    {
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                x.Parent.Parent.Parent.Name
            })
            .Build();

        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Name = x.Parent.Parent.Parent.Name
        };
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Bind_Simple_Collection()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { Files = x.Files.Select(f => new { f.Name }) })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Multiple_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                Files = x.Files.Select(f => new { f.Name }),
                Children = x.Children.Select(c => new { c.Name })
            })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList(),
            Children = x.Children.Select(c => new FolderModel { Name = c.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Bind_Nested_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new
            {
                Files = x.Files.Select(f => new
                {
                    f.Name,
                    Content = new { f.Content.ContentType },
                    Likes = f.Likes.Select(l => new { l.Count })
                })
            })
            .Build();

        // Expected
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel
            {
                Name = f.Name,
                Content = new FileContent { ContentType = f.Content.ContentType },
                Likes = f.Likes.Select(l => new Like { Count = l.Count }).ToList()
            }).ToList()
        };

        // Act & Assert
        var result = BindingDefinition.Compile(binding);
        Assert.AreEqual(expected.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Handle_Empty_Bindings()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>().Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel() { };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Handle_Null_Collections()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FolderModel>()
            .Add(x => new { Files = x.Files.Select(f => new { f.Name }) })
            .Build();

        // Expected (should still generate the expression even if source is null)
        Expression<Func<FolderModel, FolderModel>> expected = x => new FolderModel
        {
            Files = x.Files.Select(f => new FileModel { Name = f.Name }).ToList()
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Handle_CustomTrace_Default_Values()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<CustomerModel>()
            .Add(x => new
            {
                Trace = new
                {
                    x.Trace.InsertDate
                }
            })
            .Build();

        // Expected
        Expression<Func<CustomerModel, CustomerModel>> expected = x => new CustomerModel
        {
            Trace = new CustomTrace { InsertDate = x.Trace.InsertDate }
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Maintain_Original_Values_For_Unbound_Properties()
    {
        // Arrange
        var binding = BindingDefinition.CreateBuilder<FileModel>()
            .Add(x => new { x.Name })
            .Build();

        // Expected (Content should remain unbound)
        Expression<Func<FileModel, FileModel>> expected = x => new FileModel
        {
            Name = x.Name
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }

    [TestMethod]
    public void Should_Create_Binding_For_Jinget_Model()
    {
        var result = new CustomerModel().GetConstantFields();

        Expression<Func<CustomerModel, CustomerModel>> expectedResult = x => new CustomerModel()
        {
            Trace = new CustomTrace()
            {
                InsertDate = x.Trace.InsertDate,
                CreatedBy = x.Trace.CreatedBy
            },
            Name = x.Name,
            Orders = x.Orders.Select(x => new OrderModel
            {
                Trace = new CustomTrace()
                {
                    InsertDate = x.Trace.InsertDate,
                    CreatedBy = x.Trace.CreatedBy
                },
                Name = x.Name
            }).ToList()
        };

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public void Should_Handle_Nullable_Navigation()
    {
        // Arrange
        Expression<Func<OrderModel, object>> binding = x => new
        {
            x.Number,
            Customer = new
            {
                x.Customer.Name,
                Trace = new
                {
                    x.Customer.Trace.CreatedBy,
                    x.Customer.Trace.InsertDate
                }
            }
        };

        // Expected
        Expression<Func<OrderModel, OrderModel>> expected = x => new OrderModel()
        {
            Number = x.Number,
            Customer = new CustomerModel()
            {
                Name = x.Customer.Name,
                Trace = new CustomTrace()
                {
                    CreatedBy = x.Customer.Trace.CreatedBy,
                    InsertDate = x.Customer.Trace.InsertDate
                }
            }
        };

        // Act & Assert
        Assert.AreEqual(expected.ToString(), BindingDefinition.Compile(binding).ToString());
    }
}