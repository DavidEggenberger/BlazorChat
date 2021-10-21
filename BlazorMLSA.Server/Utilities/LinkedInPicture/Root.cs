using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorMLSA.Server.Utilities.LinkedInPicture
{
    public class Paging
    {
        public int count { get; set; }
        public int start { get; set; }
        public List<object> links { get; set; }
    }
    public class RawCodecSpec
    {
        public string name { get; set; }
        public string type { get; set; }
    }
    public class DisplaySize
    {
        public double width { get; set; }
        public string uom { get; set; }
        public double height { get; set; }
    }
    public class StorageSize
    {
        public int width { get; set; }
        public int height { get; set; }
    }
    public class StorageAspectRatio
    {
        public double widthAspect { get; set; }
        public double heightAspect { get; set; }
        public string formatted { get; set; }
    }
    public class DisplayAspectRatio
    {
        public double widthAspect { get; set; }
        public double heightAspect { get; set; }
        public string formatted { get; set; }
    }
    public class ComLinkedinDigitalmediaMediaartifactStillImage
    {
        public string mediaType { get; set; }
        public RawCodecSpec rawCodecSpec { get; set; }
        public DisplaySize displaySize { get; set; }
        public StorageSize storageSize { get; set; }
        public StorageAspectRatio storageAspectRatio { get; set; }
        public DisplayAspectRatio displayAspectRatio { get; set; }
    }
    public class Dataa
    {
        [JsonPropertyName("com.linkedin.digitalmedia.mediaartifact.StillImage")]
        public ComLinkedinDigitalmediaMediaartifactStillImage ComLinkedinDigitalmediaMediaartifactStillImage { get; set; }
    }
    public class Identifier
    {
        public string identifier { get; set; }
        public int index { get; set; }
        public string mediaType { get; set; }
        public string file { get; set; }
        public string identifierType { get; set; }
        public int identifierExpiresInSeconds { get; set; }
    }
    public class Element
    {
        public string artifact { get; set; }
        public string authorizationMethod { get; set; }
        public Dataa data { get; set; }
        public List<Identifier> identifiers { get; set; }
    }
    public class DisplayImage
    {
        public Paging paging { get; set; }
        public List<Element> elements { get; set; }
    }
    public class ProfilePicture
    {
        public string displayImage { get; set; }
        [JsonPropertyName("displayImage~")]
        public DisplayImage DisplayImage { get; set; }
    }
    public class Root
    {
        public ProfilePicture profilePicture { get; set; }
        public string id { get; set; }
    }
}
