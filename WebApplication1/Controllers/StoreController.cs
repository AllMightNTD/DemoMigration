using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Dtos.StoreDto;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class StoreController:ControllerBase
    {
        private readonly EmployeeContext _dbContext;
        public StoreController(EmployeeContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET api/stores
        [HttpGet("getall")]
        public ActionResult<IEnumerable<Store>> GetStores([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, [FromQuery] StoreFilterDto filter = null)
        {
            IQueryable<Store> query = _dbContext.Stores.AsQueryable();

            if (filter != null && !string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(s => s.Name.Contains(filter.Keyword) || s.Address.Contains(filter.Keyword));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            query = query.Skip(pageSize * pageIndex).Take(pageSize);

            var stores = query.ToList();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = stores
            });
        }

        // GET api/stores/{id}
        [HttpGet("{id}")]
        public ActionResult<Store> GetStore(int id)
        {
            var store = _dbContext.Stores.FirstOrDefault(s => s.ID == id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        // POST api/stores
        [HttpPost("postone")]
        public async Task<ActionResult<Store>> CreateStore(Store store)
        {
            if (await _dbContext.Stores.AnyAsync(s => s.Name == store.Name))
            {
                return BadRequest("Tên cửa hàng đã tồn tại.");
            }

            _dbContext.Stores.Add(store);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStore), new { id = store.ID }, store);
        }



        // PUT api/stores/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateStore(int id, Store updatedStore)
        {
            var store = _dbContext.Stores.FirstOrDefault(s => s.ID == id);
            if (store == null)
            {
                return NotFound();
            }

            if (_dbContext.Stores.Any(s => s.Name == updatedStore.Name && s.ID != id))
            {
                return BadRequest("Tên cửa hàng đã tồn tại.");
            }

            store.Name = updatedStore.Name;
            store.Address = updatedStore.Address;
            store.OpeningTime = updatedStore.OpeningTime;
            store.ClosingTime = updatedStore.ClosingTime;
            store.FriendlinessLevel = updatedStore.FriendlinessLevel;

            _dbContext.SaveChanges();
            return NoContent();
        }

        // DELETE api/stores/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteStore(int id)
        {
            var store = _dbContext.Stores.FirstOrDefault(s => s.ID == id);
            if (store == null)
            {
                return NotFound();
            }

            _dbContext.Stores.Remove(store);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpGet("suppliers/highest-friendliness")]
        public ActionResult<IEnumerable<SupplierDto>> GetSuppliersWithHighestFriendliness(int storeId)
        {
            var store = _dbContext.Stores.Include(s => s.Suppliers).FirstOrDefault(s => s.ID == storeId);
            if (store == null)
            {
                return NotFound();
            }

            var highestFriendlinessLevel = store.Suppliers.Max(s => s.FriendlinessLevel);
            var suppliersWithHighestFriendliness = store.Suppliers.Where(s => s.FriendlinessLevel == highestFriendlinessLevel).Select(s => new SupplierDto
            {
                Name = s.Name,
                PhoneNumber = s.PhoneNumber
            });

            return Ok(suppliersWithHighestFriendliness);
        }

    }
}
