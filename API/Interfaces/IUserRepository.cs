using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        
        // Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByUserName(string userName);
        Task<AppUser> GetUserByIdAsync(int id);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string userName);
    }
}