namespace ProductService.Domain.Exceptions;

public class ProductDomainException : Exception
{
    public ProductDomainException(string message) : base(message) { }
    public ProductDomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class ProductNotFoundException : ProductDomainException
{
    public ProductNotFoundException(Guid productId) 
        : base($"Product with ID '{productId}' was not found.") { }
    
    public ProductNotFoundException(string name, string version) 
        : base($"Product with name '{name}' and version '{version}' was not found.") { }
}

public class ProductAlreadyExistsException : ProductDomainException
{
    public ProductAlreadyExistsException(string name, string version) 
        : base($"Product with name '{name}' and version '{version}' already exists.") { }
}

public class InactiveProductException : ProductDomainException
{
    public InactiveProductException() 
        : base("Cannot perform this operation on an inactive product.") { }
    
    public InactiveProductException(string operation) 
        : base($"Cannot perform '{operation}' on an inactive product.") { }
}
