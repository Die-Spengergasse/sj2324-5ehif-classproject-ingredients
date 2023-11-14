import { ApolloServer, gql } from 'apollo-server';
import axios from 'axios';

// Define your GraphQL schema using gql
const typeDefs = gql`
  type Recipe {
    # Define the fields you want to retrieve from the Spoonacular API response
    title: String
    // Add more fields as needed
  }

  type Query {
    searchRecipes(query: String!, cuisine: String!, diet: String!): [Recipe]
  }
`;

// Define your resolver function
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
        // Map the Spoonacular API response to the shape defined in your GraphQL schema
        const recipes = response.data.results.map((result) => ({
          title: result.title,
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

// Create an instance of ApolloServer
const server = new ApolloServer({
  typeDefs,
  resolvers,
});

// Start the server
server.listen().then(({ url }) => {
  console.log(`Server ready at ${url}`);
});
