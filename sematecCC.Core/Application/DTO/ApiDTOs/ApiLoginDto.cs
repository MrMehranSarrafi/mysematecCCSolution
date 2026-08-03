namespace Application.DTO.ApiDTOs; 

//[Serializable]
public class ApiLoginDto
{
    //public int Id { get; set; }
    public string ApiUsername { get; set; }
    //[DataType(DataType.Password)]
    public string ApiPassword { get; set; }
}
