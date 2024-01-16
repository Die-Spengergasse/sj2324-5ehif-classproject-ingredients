[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/Jnp0ezuD)

## Running Neo4j Docker

Remove ```--env=NEO4J_AUTH=none``` to enable authentication. Default credentials are neo4j/neo4j.
```
docker run --name "neo4j_container" -p 7474:7474 -p 7687:7687 -d --env NEO4J_AUTH=none neo4j:latest
```

Locally accessible at: http://localhost:7474/browser/