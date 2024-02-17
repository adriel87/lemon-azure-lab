# lemon-azure-lab
ejercicio del laboratorio del bootcamp backend de lemon code 3 edicion

## Obligatorio

desde la rama main del repositorio tenemos 2 carpetas, backend y deleteJobConsoleProyect

- backend
se modifica el metodo `DeleteHero` de la clase `HeroController`. Tomando de base el metodo de ***putHero***, se hacen las modificaciones necesarias tanto en el repositorio de heroes cambiando la firma de la funcion que borra un heroe para que ahora en lugar de ser un metodo que `void` devuelva un `Hero` 
```csharp


     [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHero(int id)
        {
            Hero hero = _heroRepository.Delete(id);

            /*********** Background processs (We have to rename the image) *************/
            if (hero != null)
            {
                // Get the connection string from app settings
                string connectionString = _configuration.GetConnectionString("AzureStorage");

                // Instantiate a QueueClient which will be used to create and manipulate the queue
                var queueClient = new QueueClient(connectionString, "pics-to-delete");

                // Create a queue
                await queueClient.CreateIfNotExistsAsync();

                // Create a dynamic object to hold the message
                var message = new
                {
                    heroName = hero.Name,
                    alterEgoName = hero.AlterEgo
                };

                // Send the message
                await queueClient.SendMessageAsync(JsonSerializer.Serialize(message).ToString());

            }
            /*********** End Background processs *************/

            return NoContent();
        }
```

- deleteJobConsoleProyect

se crea un proyecto de consola en el que estamos constantemente mirando si hay mensajes en la cola mediante un bucle infinito


## optional

para ver la parte opcional del ejercicio debemos cambiar de rama

```shell
git switch optional
```

en este caso tenemos la carpeta de 

