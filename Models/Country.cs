using System.ComponentModel.DataAnnotations;

namespace FileStorage.Models
{
    /// <summary>
    /// Danh mục Quốc gia
    /// </summary>
    public class Country : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsoCode2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsoCode3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AddressFormat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? PostcodeRequired { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Default { get; set; }
    }
}
