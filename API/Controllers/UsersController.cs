using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API.Data;
using API.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
        {
            _repository = repository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            return Ok(await _repository.GetMembersAsync());
        }

        // Here attribute Name is a route name.
        [HttpGet("{userName}", Name ="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
           return await _repository.GetMemberAsync(userName);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto updateDto)
        {
            var user = await _repository.GetUserByUserName(User.GetUserName()); 
            _mapper.Map(updateDto, user);
            _repository.Update(user);
            if(await _repository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _repository.GetUserByUserName(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null ) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };
            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _repository.SaveAllAsync())
            {
                return CreatedAtRoute(nameof(GetUser), new {userName = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }          
            return BadRequest("Problem adding photos");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
           var user = await _repository.GetUserByUserName(User.GetUserName());
           var photo = user.Photos.FirstOrDefault(x => x.Id.Equals(photoId));
           if (photo.IsMain) return BadRequest("This is already your main photo.");

           var currentMain = user.Photos.FirstOrDefault();        
           if (currentMain != null) 
           { currentMain.IsMain = false; }
           photo.IsMain = true;
           if (await _repository.SaveAllAsync()) return NoContent();
           return BadRequest("Failed to set main photo.");
        } 

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _repository.GetUserByUserName(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.IsMain) return BadRequest("You can't delete your main photo.");
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _repository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete the photo.");
        }
    }
}