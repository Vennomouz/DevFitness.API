using Microsoft.AspNetCore.Mvc;
using DevFitness.API.Models.InputModels;
using DevFitness.API.Persistence;
using System.Linq;
using DevFitness.API.Models.ViewModels;
using DevFitness.API.Core.Entities;

namespace DevFitness.API.Controllers
{
    // api/users/4/meals
    [Route("api/users/{userId}/meals")]
    public class MealsController : ControllerBase
    {
        private readonly DevFitnessDBContext _dbContext;
        public MealsController(DevFitnessDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        //api/users/4/meals HTTP GET
        [HttpGet]
        public IActionResult GetAll(int userId)
        {
            var userMeals =_dbContext.Meals.Where(m => m.UserId == userId && m.Active);

            if(userMeals == null)
            {
                return NotFound();
            }

            var allMealViewModel = userMeals.Select(m => new MealViewModel(m.Id, m.Description, m.Calories, m.Date));
            return Ok(allMealViewModel);
        }

        //api/users/4/meals/7 HTTP GET Meal 7 from User 4
        [HttpGet("{mealId}")]
        public IActionResult Get(int userId, int mealId)
        {
            var userMeal = _dbContext.Meals.SingleOrDefault(m => m.Id == mealId && m.UserId == userId);

            if(userMeal == null)
            {
                return NotFound();
            }

            var mealViewModel = new MealViewModel(userMeal.Id, userMeal.Description, userMeal.Calories, userMeal.Date);
            return Ok(mealViewModel);
        }

        //api/users/4/meals HTTP POST
        [HttpPost]
        public IActionResult Post(int userId, [FromBody] CreateMealInputModel inputMeal)
        {
            var meal = new Meal(inputMeal.Description, inputMeal.Calories, inputMeal.Date, userId);

            _dbContext.Meals.Add(meal);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { userId = userId, mealId = meal.Id}, inputMeal);
        }

        //api/users/4/meals/8 HTTP PUT atualizar meal 8 from user 4
        [HttpPut("{mealId}")]
        public IActionResult Put(int userId, int mealId, [FromBody] UpdateMealInputModel updateMeal)
        {
            var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == mealId && m.UserId == userId);

            if(meal == null)
            {
                return NotFound();
            }

            meal.Update(updateMeal.Description, updateMeal.Calories, updateMeal.Date);

            _dbContext.Meals.Update(meal);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{mealId}")]
        public IActionResult Delete(int userId, int mealId)
        {
            var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == mealId && m.UserId == userId);

            if(meal == null)
            {
                return NotFound();
            }

            meal.Deactivate();

            return NoContent();
        }
    }
}
