using Golf.Core.Exceptions;
using Golf.Domain.Shared.System;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Services.SystemSettings
{
    public class SystemSettingService
    {
        private readonly SystemSettingRepository _systemSettingRepository;

        public SystemSettingService(SystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

       public bool AddSetting(Setting setting)
        {
           var sysSetting= _systemSettingRepository.FirstOrDefault();
            if(setting==null)
            {
                sysSetting = new SystemSetting();
                sysSetting.Setting = setting;
                _systemSettingRepository.Add(sysSetting);
                return true;
            }   
            else
            {
                throw new BadRequestException("Setting is exit!");
            }
        }

        /// <summary>
        /// Cập nhật cài đặt hệ thống
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool UpdateSetting(Setting setting)
        {
           var sysSetting= _systemSettingRepository.FirstOrDefault();
            if(setting==null)
            {
                sysSetting = new SystemSetting();
                sysSetting.Setting = setting;
                _systemSettingRepository.Add(sysSetting);
                return true;
            }   
            else
            {
                sysSetting.Setting = setting;
                _systemSettingRepository.Update(sysSetting);
                return true;
            }
            
        }
    }
}
