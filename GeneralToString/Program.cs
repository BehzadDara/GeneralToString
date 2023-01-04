using System.Collections;
using System.Reflection;
using System.Text;

var testObject = new TestClass
{
    Age = 24,
    Name = "behzad",
    PhoneNumbers = new() {"09128584936", "09208584936"},
    Scores= new() { 1, -0.7, 7},
    Info = new InnerClass
    {
        HasCar = true,
        Description = "some description",
        CarsInfo = new List<InnerInnerClass> { 
            new InnerInnerClass
            {
                FullInfo = "first"
            },
            new InnerInnerClass
            {
                FullInfo = "second"
            }
        }
    }
};

var testList = new List<TestClass>
{
    testObject,
    testObject
};

Console.WriteLine(testObject.GeneralToString());


public class TestClass : IGeneralToString
{
    public int Age { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new List<string>();
    public List<double> Scores { get; set; } = new List<double>();
    public InnerClass Info { get; set; } = new();
}

public class InnerClass : IGeneralToString
{
    public bool HasCar { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<InnerInnerClass> CarsInfo { get; set; } = new();
}

public class InnerInnerClass : IGeneralToString
{
    public string FullInfo { get; set; } = string.Empty;
}

public interface IGeneralToString
{

}

public static class ExtentionMethod
{
    public static string GeneralToString<T>(this T obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var sb = new StringBuilder();
        sb.AppendLine();

        sb.AppendLine($"object of type {obj.GetType().Name} is:");

        if (obj is IList list)
        {
            foreach (var item in list)
            {
                foreach (var propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    sb.AppendLine($"property {propertyInfo.Name} of type {propertyInfo.PropertyType.Name} is [{PrintValue(item, propertyInfo)}],");
                }
            }
        }
        else
        {
            foreach (var propertyInfo in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                sb.AppendLine($"property {propertyInfo.Name} of type {propertyInfo.PropertyType.Name} is [{PrintValue(obj, propertyInfo)}],");
            }
        }

        return sb.ToString();
    }

    private static object PrintValue(object obj, PropertyInfo propertyInfo)
    {
        if(propertyInfo.GetValue(obj) is IGeneralToString)
        {
            return propertyInfo.GetValue(obj).GeneralToString();
        }

        if(propertyInfo.GetValue(obj) is IList list)
        {
            var sb = new StringBuilder();

            foreach (var item in list)
            {
                if (item is IGeneralToString)
                {
                    sb.Append(item.GeneralToString());
                }
                else
                    sb.Append($"{item}, ");
            }
            return sb.ToString();
        }
        return propertyInfo.GetValue(obj) ?? new();
    }
}