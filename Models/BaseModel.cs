using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorage.Models
{
    public class BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseModel()
        {
            Active = true;
            ModifiedAt = DateTimeOffset.UtcNow; // Get current time
        }
        /// <summary>
        /// Id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public int Id { get; set; }
        /// <summary>
        /// Tài khoản tạo
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Tài khoản cập nhật gần nhất
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
        /// <summary>
        /// Thời gian cập nhật gần nhất
        /// </summary>
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
