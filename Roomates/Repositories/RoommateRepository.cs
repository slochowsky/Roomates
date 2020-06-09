using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate FROM Roommate";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int IdColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(IdColumnPosition);

                        int FirstNameColumn = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumn);

                        int LastNameColumn = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumn);

                        int RentColumnPosition = reader.GetOrdinal("RentPortion");
                        int RentPortion = reader.GetInt32(RentColumnPosition);

                        int DateColumnPos = reader.GetOrdinal("MoveInDate");
                        DateTime MoveIn = reader.GetDateTime(DateColumnPos);

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = FirstNameValue,
                            Lastname = LastNameValue,
                            RentPortion = RentPortion,
                            MovedInDate = MoveIn,
                            Room = null

                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }
            }
        }

        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstName, LastName, RentPortion, MoveInDate FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            Firstname = reader.GetString(reader.GetOrdinal("FirstName")),
                            Lastname = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = null
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }

        public List<Roommate> GetAllWithRoom()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.Id AS roommateId, FirstName, LastName, RentPortion, MoveInDate, Name, MaxOccupancy, roomId 
                                        FROM Roommate rm 
                                        JOIN Room room ON room.Id = rm.RoomId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int IdColumnPosition = reader.GetOrdinal("roommateId");
                        int idValue = reader.GetInt32(IdColumnPosition);

                        int FirstNameColumn = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumn);

                        int LastNameColumn = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumn);

                        int RentColumnPosition = reader.GetOrdinal("RentPortion");
                        int RentPortion = reader.GetInt32(RentColumnPosition);

                        int DateColumnPos = reader.GetOrdinal("MoveInDate");
                        DateTime MoveIn = reader.GetDateTime(DateColumnPos);

                        int roomId = reader.GetInt32(reader.GetOrdinal("roomId"));
                        int roomNamePos = reader.GetOrdinal("Name");

                        string RoomName = reader.GetString(roomNamePos);
                        int MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"));

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = FirstNameValue,
                            Lastname = LastNameValue,
                            RentPortion = RentPortion,
                            MovedInDate = MoveIn,
                            Room = new Room
                            {
                                Id = roomId,
                                Name = RoomName,
                                MaxOccupancy = MaxOccupancy
                            }

                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }

            }
        }

        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@firstName, @lastName, @rentPortion, @moveInDate, @roomId)";
                    cmd.Parameters.AddWithValue("@firstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@lastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@moveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@roomId", roommate.Room.Id);
                    int id = (int)cmd.ExecuteScalar();

                    roommate.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        public void Update(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Roommate
                                    SET FirstName = @firstName,
                                        LastName = @lastName,
                                        RentPortion = @rentPortion,
                                        MoveInDate = @MoveInDate,
                                        RoomId = @roomId
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@firstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@lastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@RoomId", roommate.Room.Id);
                    cmd.Parameters.AddWithValue("@id", roommate.Id);

                    cmd.ExecuteNonQuery();
                }
            }


        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
