import { gql } from 'apollo-server';
import axios from 'axios';

const typeDefs = gql`
  type Recipe {
    id: ID!
    title: String
    image: String
    ingredients: [String]
    instructions: String
    # Add more fields as needed
  }

  type Query {
    searchRecipes(
      query: String!
      cuisine: String!
      diet: String!
    ): [Recipe]
  }
`;

const resolvers = {
  Query: {
    searchRecipes: async (_, { query, cuisine, diet }) => {
      const options = {
        method: 'GET',
        url: 'https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/complexSearch',
        params: {
          query,
          cuisine,
          diet,
          // Add more parameters as needed
        },
        headers: {
          'X-RapidAPI-Key': '3e20366858msh83cd201175488e0p12db60jsn68c137092f39',
          'X-RapidAPI-Host': 'spoonacular-recipe-food-nutrition-v1.p.rapidapi.com',
        },
      };

      try {
        const response = await axios.request(options);
        const recipes = response.data.results.map((result) => ({
          id: result.id,
          title: result.title,
          image: result.image,
          ingredients: result.extendedIngredients.map((ingredient) => ingredient.name),
          instructions: result.instructions,
          // Map other fields as needed
        }));

        return recipes;
      } catch (error) {
        console.error(error);
        throw new Error('Unable to fetch recipes');
      }
    },
  },
};

export { typeDefs, resolvers };
