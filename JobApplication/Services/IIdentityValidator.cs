using JobApplicationLibrary.Models;

namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string number);
        ICountryProvider CountryDataProvider { get; }


        public ValidationMode ValidationMode { get; set; }

    }
    public enum ValidationMode
    {
        Quick,
        Detailed
    }

    public interface ICountryData
    {
        string Country { get;}
    }

    public interface ICountryProvider
    {
        ICountryData CountryData { get;}
    }

}