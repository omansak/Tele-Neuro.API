using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.BrochureService.Models;

namespace TeleNeuro.Service.BrochureService
{
    public class BrochureService : IBrochureService
    {
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public BrochureService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Returns Exercises
        /// </summary>
        /// <returns></returns>
        public async Task<(List<BrochureInfo> Result, int Count)> ListExercises(PageInfo pageInfo = null)
        {
            return (await GetQueryableBrochure(pageInfo: pageInfo).ToListAsync(), await GetQueryableBrochure().CountAsync());
        }

        /// <summary>
        /// Return Exercise
        /// </summary>
        public async Task<BrochureInfo> GetBrochure(int id)
        {
            return await GetQueryableBrochure(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Insert or update Brochure (CreatedDate can not modify)
        /// </summary>
        public async Task<BrochureInfo> UpdateBrochure(Brochure brochure)
        {
            if (brochure.Id > 0)
            {
                var exerciseRow = await _baseRepository.SingleOrDefaultAsync<Brochure>(i => i.Id == brochure.Id);
                if (exerciseRow != null)
                {
                    exerciseRow.Name = brochure.Name;
                    exerciseRow.IsActive = brochure.IsActive;
                    exerciseRow.CreatedDate = DateTime.Now;
                    exerciseRow.CreatedUser = brochure.CreatedUser;
                    if (!string.IsNullOrWhiteSpace(brochure.DocumentGuid))
                        exerciseRow.DocumentGuid = brochure.DocumentGuid;

                    var result = await _baseRepository.UpdateAsync(exerciseRow);
                    return await GetBrochure(result.Id);
                }
                throw new UIException("Broşür bulunamadi");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(brochure.DocumentGuid))
                    throw new UIException("Döküman boş olamaz");

                brochure.CreatedDate = System.DateTime.Now;
                var result = await _baseRepository.InsertAsync(new Brochure
                {
                    Name = brochure.Name,
                    IsActive = true,
                    CreatedDate = System.DateTime.Now,
                    DocumentGuid = brochure.DocumentGuid,
                    CreatedUser = brochure.CreatedUser
                });
                return await GetBrochure(result.Id);
            }
        }

        /// <summary>
        /// Toggle Brochure IsActive Status
        /// </summary>
        public async Task<bool> ToggleExerciseStatus(int exerciseId)
        {
            var brochureRow = await _baseRepository.SingleOrDefaultAsync<Brochure>(i => i.Id == exerciseId);
            if (brochureRow != null)
            {
                brochureRow.IsActive = !brochureRow.IsActive;
                await _baseRepository.UpdateAsync(brochureRow);
                return true;
            }

            throw new UIException("Broşür bulunamadi");
        }

        /// <summary>
        /// Assign user to Broşür
        /// </summary>
        public async Task<int> AssignUser(AssignBrochureUserModel model)
        {
            var user = await _baseRepository.SingleOrDefaultAsync<User>(i => i.IsActive && i.Id == model.UserId);
            if (user == null)
                throw new UIException("Kullanıcı bulunamadı");

            var brochure = await _baseRepository.SingleOrDefaultAsync<Brochure>(i => i.IsActive && i.Id == model.BrochureId);
            if (brochure == null)
                throw new UIException("Broşür bulunamadı");

            var relational = await _baseRepository.SingleOrDefaultAsync<UserBrochureRelation>(i => i.BrochureId == brochure.Id && i.UserId == user.Id);

            if (relational != null)
                throw new UIException("Broşür zaten kullanıcıya atanmış");

            var brochureUserRelational = await _baseRepository.InsertAsync<UserBrochureRelation>(new UserBrochureRelation
            {
                CreatedDate = DateTime.Now,
                CreatedUser = model.AssignedUserId,
                BrochureId = brochure.Id,
                UserId = user.Id
            });

            if (brochureUserRelational?.Id > 0)
                return brochureUserRelational.Id;

            throw new UIException("Broşür kullanıcıya atanamadı");
        }

        /// <summary>
        /// Delete a relation of user of program relation
        /// </summary>
        public async Task<bool> DeleteAssignedUser(AssignBrochureUserModel model)
        {
            var user = await _baseRepository.SingleOrDefaultAsync<User>(i => i.IsActive && i.Id == model.UserId);
            if (user == null)
                throw new UIException("Kullanıcı bulunamadı");

            var brochure = await _baseRepository.SingleOrDefaultAsync<Brochure>(i => i.IsActive && i.Id == model.BrochureId);
            if (brochure == null)
                throw new UIException("Broşür bulunamadı");

            var brochureUserRelational = await _baseRepository.SingleOrDefaultAsync<UserProgramRelation>(i => i.ProgramId == brochure.Id && i.UserId == user.Id);

            if (brochureUserRelational != null)
            {
                await _baseRepository.DeleteAsync(brochureUserRelational);
                return true;
            }
            throw new UIException("Atama bulunamadı");
        }

        /// <summary>
        /// List assigned users of program
        /// </summary>
        public async Task<(List<AssignedBrochureUserInfo>, int)> ListAssignedUsers(AssignedBrochureUserModel model)
        {
            var brochureRow = await _baseRepository.SingleOrDefaultAsync<Brochure>(i => i.Id == model.BrochureId);
            if (brochureRow != null)
            {
                var query = _baseRepository
                    .GetQueryable<UserBrochureRelation>()
                    .Join(_baseRepository.GetQueryable<User>(), i => i.UserId, j => j.Id, (i, j) => new
                    {
                        User = j,
                        Relation = i
                    })
                    .Join(_baseRepository.GetQueryable<UserProfile>(), i => i.User.Id, j => j.UserId, (i, j) => new
                    {
                        User = i.User,
                        Relation = i.Relation,
                        UserProfile = j
                    })
                    .Where(i => i.Relation.BrochureId == brochureRow.Id);

                var resultQuery = query
                    .Skip((model.PageInfo.Page - 1) * model.PageInfo.PageSize)
                    .Take(model.PageInfo.PageSize)
                    .Select(i => new AssignedBrochureUserInfo
                    {
                        UserId = i.User.Id,
                        Name = i.UserProfile.Name,
                        Surname = i.UserProfile.Surname,
                        Email = i.User.Email,
                        RelationId = i.Relation.Id
                    });
                return (await resultQuery.ToListAsync(), await query.CountAsync());
            }
            throw new UIException("Broşür bulunamadı");
        }

        /// <summary>
        /// List assigned programs of users
        /// </summary>
        public async Task<(List<AssignedBrochureOfUserInfo>, int)> ListAssignedBrochures(AssignedBrochureOfUserModel model)
        {
            var userRow = await _baseRepository.GetQueryable<User>().SingleOrDefaultAsync(i => i.Id == model.UserId);
            if (userRow != null)
            {
                var query = _baseRepository
                    .GetQueryable<UserBrochureRelation>()
                    .Join(_baseRepository.GetQueryable<Brochure>(), i => i.BrochureId, j => j.Id, (i, j) => new
                    {
                        Relation = i,
                        Brochure = j
                    })
                    .GroupJoin(_baseRepository.GetQueryable<Document>(), i => i.Brochure.DocumentGuid, j => j.Guid, (i, j) => new
                    {
                        Brochure = i.Brochure,
                        Relation = i.Relation,
                        Document = j
                    })
                    .SelectMany(i => i.Document.DefaultIfEmpty(), (i, j) => new
                    {
                        Brochure = i.Brochure,
                        Relation = i.Relation,
                        Document = j
                    })
                    .Where(i => i.Relation.UserId == userRow.Id && i.Brochure.IsActive);

                var resultQuery = query
                    .OrderByDescending(i => i.Relation.CreatedDate)
                    .Skip((model.PageInfo.Page - 1) * model.PageInfo.PageSize)
                    .Take(model.PageInfo.PageSize)
                    .Select(i => new AssignedBrochureOfUserInfo
                    {
                        BrochureId = i.Brochure.Id,
                        BrochureName = i.Brochure.Name,
                        Document = i.Document,
                        AssignDate = i.Relation.CreatedDate
                    });

                return (await resultQuery.ToListAsync(), await query.CountAsync());
            }
            throw new UIException("Kullanıcı bulunamadı");
        }

        /// <summary>
        /// Return BrochureInfo Queryable
        /// </summary>
        private IQueryable<BrochureInfo> GetQueryableBrochure(Expression<Func<Brochure, bool>> expression = null, PageInfo pageInfo = null)
        {
            var query = _baseRepository.GetQueryable<Brochure>().AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            var queryableCategory = query
                .OrderByDescending(i => i.IsActive)
                .ThenByDescending(i => i.CreatedDate)
                .GroupJoin(_baseRepository.GetQueryable<Document>(), i => i.DocumentGuid, j => j.Guid, (i, j) => new
                {
                    Brochure = i,
                    Document = j
                })
                .SelectMany(i => i.Document.DefaultIfEmpty(), (i, j) => new BrochureInfo
                {
                    Brochure = i.Brochure,
                    Document = j
                });

            if (pageInfo != null)
            {
                queryableCategory = queryableCategory
                    .Skip((pageInfo.Page - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);
            }
            return queryableCategory;
        }
    }
}
