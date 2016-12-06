Add-Migration -Name $Name -ConnectionString "Data Source=localhost;Initial Catalog=CodeGolf;Integrated Security=True" -ConnectionProviderName "System.Data.SqlClient" 
Update-Database -ConnectionString "Data Source=localhost;Initial Catalog=CodeGolf;Integrated Security=True" -ConnectionProviderName "System.Data.SqlClient" 
