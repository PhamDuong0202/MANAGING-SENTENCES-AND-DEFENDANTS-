using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
namespace DuongBanAn
{
    public class Program
    {
        static void Main(string[] args)
        {
            ClientContext context = new ClientContext("http://sp-31:3005/thuctap");
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            #region authenticate

            context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
            context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo("Quantri.ctt", "123456a@");

            #endregion
            Web webthuctap = context.Web;
            context.Load(webthuctap);
            context.ExecuteQuery();

            string tenweb = webthuctap.Title;
            //Lấy danh sách bị cáo
            List lst = webthuctap.GetList("/thuctap/Lists/Duong_BiCao");
            context.Load(lst);
            context.ExecuteQuery();

            //Lấy danh sách các quốc tịch đã có
            List nationalityList = webthuctap.GetList("/thuctap/Lists/QuocTich");
            context.Load(nationalityList);
            context.ExecuteQuery();

            //Lấy danh sách địa chỉ (tỉnh)
            List addressList = webthuctap.GetList("/thuctap/Lists/DiaChi");
            context.Load(addressList);
            context.ExecuteQuery();

            //Lấy danh sách bản án
            List bananList = webthuctap.GetList("/thuctap/Lists/Duong_BanAn");
            context.Load(bananList);
            context.ExecuteQuery();
            //Lấy danh sách hình ảnh
            /*List imageList = webthuctap.GetList("/thuctap/Lists/PublishingImages");
            context.Load(imageList);
            context.ExecuteQuery();*/
            //Form 
            while (true)
            {
                Console.WriteLine("--------Dương Share Point---------");
                Console.WriteLine("1.Bị cáo ");
                Console.WriteLine("2.Bản án");
                Console.WriteLine("0.Thoát");
                Console.WriteLine("Lựa chọn của bạn: ");
                int option;
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 0:
                            Console.WriteLine("Tạm biệt!");
                            Environment.Exit(0);
                            return;
                        case 1:
                            Console.WriteLine("Duong_BiCao: ");
                            Console.WriteLine("1.Thêm bị cáo");
                            Console.WriteLine("2.Sửa bị cáo");
                            Console.WriteLine("3.Xóa bị cáo");
                            Console.WriteLine("4.Danh sách các bị cáo");
                            Console.WriteLine("0.Thoát");
                            Console.WriteLine("Nhập lựa chọn:");
                            int biCaoOption;
                            if (int.TryParse(Console.ReadLine(), out biCaoOption))
                            {
                                switch (biCaoOption)
                                {
                                    case 1:
                                        Console.WriteLine("Nhập thông tin bị Cáo:");
                                        string fullName;
                                        do
                                        {
                                            Console.WriteLine("Tên bị cáo:");
                                            fullName = Console.ReadLine().Trim();
                                            if (string.IsNullOrEmpty(fullName))
                                            {
                                                Console.WriteLine("Tên bị cáo không được để trống. Vui lòng nhập lại.");
                                            }
                                        } while (string.IsNullOrEmpty(fullName));
                                        string otherName;
                                        do
                                        {
                                            Console.WriteLine("Tên gọi khác:");
                                            otherName = Console.ReadLine().Trim();
                                            if (string.IsNullOrEmpty(otherName))
                                            {
                                                Console.WriteLine("Tên gọi khác không được để trống. Vui lòng nhập lại.");
                                            }
                                        } while (string.IsNullOrEmpty(otherName));


                                        DateTime birthDate;
                                        bool isValidDate;
                                        do
                                        {
                                            Console.WriteLine("Ngày sinh (MM/dd/yyyy):");
                                            string inputDate = Console.ReadLine().Trim(); // Loại bỏ khoảng trắng không mong muốn
                                            isValidDate = DateTime.TryParseExact(inputDate, "M/d/yyyy", null, DateTimeStyles.None, out birthDate); // Sử dụng đúng định dạng ngày tháng
                                            if (!isValidDate)
                                            {
                                                Console.WriteLine("Ngày sinh không hợp lệ. Vui lòng nhập lại theo định dạng MM/dd/yyyy.");
                                            }
                                        } while (!isValidDate);

                                        Console.WriteLine("Danh sách quốc tịch:");
                                        Dictionary<int, string> nationality = DisplayListItems(nationalityList);
                                        int parsedQuocTichID;
                                        bool isQuocTichValid = false;
                                        do
                                        {
                                            Console.WriteLine("Nhập ID quốc tịch từ danh sách trên:");
                                            string inputQuocTichID = Console.ReadLine();
                                            if (int.TryParse(inputQuocTichID, out parsedQuocTichID))
                                            {
                                                // Kiểm tra xem ID quốc tịch có trong danh sách quốc tịch không
                                                if (nationality.ContainsKey(parsedQuocTichID))
                                                {
                                                    isQuocTichValid = true;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("ID quốc tịch không hợp lệ. Vui lòng nhập lại.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại.");
                                            }
                                        } while (!isQuocTichValid);

                                        // Hiển thị danh sách địa chỉ cho người dùng
                                        Console.WriteLine("Danh sách địa chỉ:");
                                        Dictionary<int, string> diaChiDict = DisplayListItems(addressList);
                                        int parsedDiaChiID;
                                        bool isDiaChiValid = false;
                                        do
                                        {
                                            Console.WriteLine("Nhập ID địa chỉ từ danh sách trên:");
                                            string inputDiaChiID = Console.ReadLine();
                                            if (int.TryParse(inputDiaChiID, out parsedDiaChiID))
                                            {
                                                // Kiểm tra xem ID địa chỉ có trong danh sách địa chỉ không
                                                if (diaChiDict.ContainsKey(parsedDiaChiID))
                                                {
                                                    isDiaChiValid = true;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("ID địa chỉ không hợp lệ. Vui lòng nhập lại.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại.");
                                            }
                                        } while (!isDiaChiValid);


                                        string fatherName;
                                        do
                                        {
                                            Console.WriteLine("Hotencha:");
                                            fatherName = Console.ReadLine().Trim();
                                            if (string.IsNullOrEmpty(fatherName))
                                            {
                                                Console.WriteLine("Tên cha không được để trống. Vui lòng nhập lại.");
                                            }
                                        } while (string.IsNullOrEmpty(fatherName));


                                        string motherName;
                                        do
                                        {
                                            Console.WriteLine("Hotenme:");
                                            motherName = Console.ReadLine().Trim();
                                            if (string.IsNullOrEmpty(motherName))
                                            {
                                                Console.WriteLine("Tên mẹ không được để trống. Vui lòng nhập lại.");
                                            }
                                        } while (string.IsNullOrEmpty(motherName));
                                        ListItemCreationInformation newTopicInfo = new ListItemCreationInformation();
                                        ListItem oListItem = lst.AddItem(newTopicInfo);
                                        //image
                                        Console.WriteLine("Ảnh đại diện");
                                        string imagePath;
                                        Microsoft.SharePoint.Client.Folder folder = webthuctap.Lists.GetByTitle("Images").RootFolder;
                                        Microsoft.SharePoint.Client.FileCollection files = folder.Files;
                                        context.Load(files);
                                        context.ExecuteQuery();
                                        bool fileExists = false;
                                        do
                                        {
                                            Console.WriteLine("Nhập đường dẫn của hình ảnh:");
                                            imagePath = Console.ReadLine();

                                            if (string.IsNullOrWhiteSpace(imagePath))
                                            {
                                                Console.WriteLine("Ảnh không được để trống. Vui lòng nhập lại.");
                                                continue;
                                            }

                                            if (!System.IO.File.Exists(imagePath))
                                            {
                                                Console.WriteLine("Đường dẫn không hợp lệ hoặc tệp không tồn tại. Vui lòng nhập lại.");
                                                continue;
                                            }

                                            string fileName = Path.GetFileName(imagePath);
                                            fileExists = files.Any(file => file.Name == fileName);
                                            if (fileExists)
                                            {
                                                Console.WriteLine("Ảnh đã tồn tại trong thư mục đích. Vui lòng nhập lại.");
                                            }
                                        }
                                        while (string.IsNullOrWhiteSpace(imagePath) || !System.IO.File.Exists(imagePath) || fileExists);

                                        Microsoft.SharePoint.Client.FileCreationInformation imageFileInfo = new Microsoft.SharePoint.Client.FileCreationInformation();
                                        imageFileInfo.Content = System.IO.File.ReadAllBytes(imagePath);
                                        imageFileInfo.Url = Path.GetFileName(imagePath);
                                        Microsoft.SharePoint.Client.Folder targetFolder = webthuctap.Lists.GetByTitle("Images").RootFolder;
                                        Microsoft.SharePoint.Client.File uploadedFile = targetFolder.Files.Add(imageFileInfo);
                                        context.Load(uploadedFile);
                                        context.ExecuteQuery();
                                        string imageName = Path.GetFileName(imagePath);

                                        // Gán giá trị cho các trường
                                        oListItem["TenBiCao"] = fullName;
                                        oListItem["TenKhac"] = otherName;
                                        oListItem["NgaySinh"] = birthDate;
                                        oListItem["QuocTich"] = parsedQuocTichID;
                                        oListItem["DiaChi"] = parsedDiaChiID;
                                        oListItem["HoTenCha"] = fatherName;
                                        oListItem["HoTenMe"] = motherName;
                                        oListItem["Avatar"] = "http://sp-31:3005/thuctap/PublishingImages/" + imageName + "?RenditionID=1";
                                        oListItem.Update();
                                        context.ExecuteQuery();

                                        Console.WriteLine("Bị cáo đã được thêm vào danh sách thành công.");
                                        break;
                                    case 2:
                                        Console.WriteLine("Sửa thông tin bị cáo!");
                                        Console.WriteLine("Nhập ID của bị cáo bạn muốn sửa:");
                                        int bicaoIdToEdit;
                                        if (int.TryParse(Console.ReadLine(), out bicaoIdToEdit))
                                        {
                                            CamlQuery query = CamlQuery.CreateAllItemsQuery();
                                            ListItemCollection items = lst.GetItems(query);
                                            context.Load(items);
                                            context.ExecuteQuery();
                                            bool isIDfound = false;
                                            foreach (ListItem item in items)
                                            {
                                                if ((int)item["ID"] == bicaoIdToEdit)
                                                {
                                                    // Nếu ID tồn tại trong danh sách, đặt isIDfound thành true và thoát khỏi vòng lặp
                                                    isIDfound = true;
                                                    break;
                                                }
                                            }
                                            if (isIDfound)
                                            {
                                                string newFullName;
                                                do
                                                {
                                                    Console.WriteLine("Nhập tên bị cáo mới:");
                                                    newFullName = Console.ReadLine().Trim();
                                                    if (string.IsNullOrEmpty(newFullName))
                                                    {
                                                        Console.WriteLine("Tên bị cáo không được để trống. Vui lòng nhập lại.");
                                                    }
                                                } while (string.IsNullOrEmpty(newFullName));
                                                
                                                
                                                Console.WriteLine("Tên khác:");
                                                string newOtherName = Console.ReadLine().Trim();
                                                DateTime newBirthDate;
                                                do
                                                {
                                                    Console.WriteLine("Ngày sinh (MM/dd/yyyy):");
                                                    string inputDate = Console.ReadLine().Trim(); 
                                                    isValidDate = DateTime.TryParseExact(inputDate, "M/d/yyyy", null, DateTimeStyles.None, out newBirthDate); // Sử dụng đúng định dạng ngày tháng
                                                    if (!isValidDate)
                                                    {
                                                        Console.WriteLine("Ngày sinh không hợp lệ. Vui lòng nhập lại theo định dạng MM/dd/yyyy.");
                                                    }
                                                } while (!isValidDate);
                                                int newQuocTichID;
                                                Dictionary<int, string> newNationality = DisplayListItems(nationalityList);
                                                bool isNewQuocTichValid = false;
                                                do
                                                {
                                                    Console.WriteLine("Nhập ID quốc tịch từ danh sách trên:");
                                                    string inputQuocTichID = Console.ReadLine();
                                                    if (int.TryParse(inputQuocTichID, out newQuocTichID))
                                                    {
                                                        // Kiểm tra xem ID quốc tịch có trong danh sách quốc tịch không
                                                        if (newNationality.ContainsKey(newQuocTichID))
                                                        {
                                                            isNewQuocTichValid = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("ID quốc tịch không hợp lệ. Vui lòng nhập lại.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại.");
                                                    }
                                                } while (!isNewQuocTichValid);

                                                // Hiển thị danh sách địa chỉ cho người dùng
                                                Console.WriteLine("Danh sách địa chỉ:");
                                                Dictionary<int, string> newdiaChiDict = DisplayListItems(addressList);
                                                int newDiaChiID;
                                                bool isNewDiaChiValid = false;
                                                do
                                                {
                                                    Console.WriteLine("Nhập ID địa chỉ từ danh sách trên:");
                                                    string inputDiaChiID = Console.ReadLine();
                                                    if (int.TryParse(inputDiaChiID, out newDiaChiID))
                                                    {
                                                        // Kiểm tra xem ID địa chỉ có trong danh sách địa chỉ không
                                                        if (newdiaChiDict.ContainsKey(newDiaChiID))
                                                        {
                                                            isNewDiaChiValid = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("ID địa chỉ không hợp lệ. Vui lòng nhập lại.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại.");
                                                    }
                                                } while (!isNewDiaChiValid);


                                                string newFatherName;
                                                do
                                                {
                                                    Console.WriteLine("Họ tên cha:");
                                                    newFatherName = Console.ReadLine().Trim();
                                                    if (string.IsNullOrEmpty(newFatherName))
                                                    {
                                                        Console.WriteLine("Tên cha không được để trống. Vui lòng nhập lại.");
                                                    }
                                                } while (string.IsNullOrEmpty(newFatherName));


                                                string newMotherName;
                                                do
                                                {
                                                    Console.WriteLine("Họ tên mẹ:");
                                                    newMotherName = Console.ReadLine().Trim();
                                                    if (string.IsNullOrEmpty(newMotherName))
                                                    {
                                                        Console.WriteLine("Tên mẹ không được để trống. Vui lòng nhập lại.");
                                                    }
                                                } while (string.IsNullOrEmpty(newMotherName));

                                                Dictionary<string, object> newFieldValues = new Dictionary<string, object> { { "TenBiCao", newFullName }, {"TenKhac", newOtherName }, { "NgaySinh", newBirthDate}, { "QuocTich", newQuocTichID }, { "DiaChi", newDiaChiID }, { "HoTenCha", newFatherName }, { "HoTenMe", newMotherName } };
                                                UpdateListItemById(lst, bicaoIdToEdit, newFieldValues);
                                            }
                                            else
                                            {
                                                Console.WriteLine("ID không hợp lệ hoặc không tồn tại!");
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("ID không hợp lệ. Vui lòng nhập một số nguyên.");
                                        }
                                        break;
                                    case 3:
                                        Console.WriteLine("Nhập ID bị cáo muốn xóa: ");
                                        int IDBiCaoDelete;
                                        if (int.TryParse(Console.ReadLine(), out IDBiCaoDelete))
                                        {
                                            CamlQuery query = CamlQuery.CreateAllItemsQuery();
                                            ListItemCollection items = lst.GetItems(query);
                                            context.Load(items);
                                            context.ExecuteQuery();
                                            bool isIDfound = false;
                                            foreach (ListItem item in items)
                                            {
                                                if ((int)item["ID"] == IDBiCaoDelete)
                                                {
                                                    // Nếu ID tồn tại trong danh sách, đặt isIDfound thành true và thoát khỏi vòng lặp
                                                    isIDfound = true;
                                                    break;
                                                }
                                            }
                                            if (isIDfound)
                                            {
                                                DeleteListItemById(lst, IDBiCaoDelete);
                                            }
                                            else
                                            {
                                                Console.WriteLine("ID không hợp lệ hoặc không tồn tại!");
                                            }
                                        }

                                        break;
                                    case 4:
                                        Console.WriteLine("------Danh sách các bị cáo-------");
                                        Console.WriteLine("| {0,-3} | {1,-16} | {2,-12} | {3,-12} | {4,-10} | {5,-12} | {6,-16} | {7,-16} |",
                                                           "ID", "Tên Bị Cáo", "Tên khác", "Ngày Sinh", "Quốc Tịch", "Địa Chỉ", "Họ Tên Cha", "Họ Tên Mẹ");
                                        PhanTrang(lst);
                                        Console.WriteLine();
                                        break;
                                }
                            }
                            break;
                        case 2:
                            Console.WriteLine("Duong_BanAn: ");
                            Console.WriteLine("1.Thêm bản án");
                            Console.WriteLine("2.Sửa bản án");
                            Console.WriteLine("3.Xóa bản án");
                            Console.WriteLine("4.Danh sách các bản án");
                            Console.WriteLine("0.Thoát");
                            Console.WriteLine("Nhập lựa chọn:");
                            int banAnoption;
                            if (int.TryParse(Console.ReadLine(), out banAnoption))
                            {
                                switch (banAnoption)
                                {
                                    case 1:
                                        Console.WriteLine("Nhập thông tin Bản án:");

                                        // Hiển thị danh sách Bị cáo để người dùng chọn
                                        Console.WriteLine("Danh sách Bị cáo:");
                                        Dictionary<int, string> bicaoDict = Hienthibicao(lst);
                                        int selectedBicaoId;
                                        bool isBicaoValid = false;
                                        do
                                        {
                                            Console.WriteLine("Nhập ID Bị cáo từ danh sách trên:");
                                            string inputBicaoId = Console.ReadLine();
                                            if (int.TryParse(inputBicaoId, out selectedBicaoId))
                                            {
                                                // Kiểm tra xem ID Bị cáo có trong danh sách không
                                                if (bicaoDict.ContainsKey(selectedBicaoId))
                                                {
                                                    isBicaoValid = true;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("ID Bị cáo không hợp lệ. Vui lòng nhập lại.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại.");
                                            }
                                        } while (!isBicaoValid);

                                        Console.WriteLine("Tên bản án:");
                                        string tenBanAn = Console.ReadLine().Trim();

                                        Console.WriteLine("Ngày hiệu lực (MM/dd/yyyy):");
                                        DateTime ngayHieuLuc;
                                        bool isValidDate = false;
                                        do
                                        {
                                            string inputDate = Console.ReadLine().Trim();
                                            isValidDate = DateTime.TryParseExact(inputDate, "M/d/yyyy", null, DateTimeStyles.None, out ngayHieuLuc);
                                            if (!isValidDate)
                                            {
                                                Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại theo định dạng MM/dd/yyyy.");
                                            }
                                        } while (!isValidDate);

                                        Console.WriteLine("Mô tả:");
                                        string moTa = Console.ReadLine().Trim();

                                        // Gán giá trị cho các trường
                                        ListItemCreationInformation newBanAnInfo = new ListItemCreationInformation();
                                        ListItem newBanAnItem = bananList.AddItem(newBanAnInfo);
                                        newBanAnItem["ID_Bicao"] = selectedBicaoId;
                                        newBanAnItem["TenBanAn"] = tenBanAn;
                                        newBanAnItem["NgayHieuLuc"] = ngayHieuLuc;
                                        newBanAnItem["MoTa"] = moTa;

                                        newBanAnItem.Update();
                                        context.ExecuteQuery();

                                        Console.WriteLine("Bản án đã được thêm vào danh sách thành công.");
                                        break;
                                    case 2:
                                        Console.WriteLine("Nhập ID của bản án bạn muốn sửa:");
                                        int banAnIdToEdit;
                                        if (int.TryParse(Console.ReadLine(), out banAnIdToEdit))
                                        {
                                            CamlQuery query = CamlQuery.CreateAllItemsQuery();
                                            ListItemCollection items = bananList.GetItems(query);
                                            context.Load(items);
                                            context.ExecuteQuery();
                                            bool isIDfound = false;
                                            foreach (ListItem item in items)
                                            {
                                                if ((int)item["ID"] == banAnIdToEdit)
                                                {
                                                    // Nếu ID tồn tại trong danh sách, đặt isIDfound thành true và thoát khỏi vòng lặp
                                                    isIDfound = true;
                                                    break;
                                                }
                                            }
                                            if (isIDfound)
                                            {
                                                Console.WriteLine("Nhập tên bản án mới:");
                                                string newTenBanAn = Console.ReadLine().Trim();

                                                if (!string.IsNullOrEmpty(newTenBanAn))
                                                {
                                                    Console.WriteLine("Nhập ngày hiệu lực mới (MM/dd/yyyy):");
                                                    DateTime newNgayHieuLuc;
                                                    do
                                                    {
                                                        string inputDate = Console.ReadLine().Trim();
                                                        isValidDate = DateTime.TryParseExact(inputDate, "M/d/yyyy", null, DateTimeStyles.None, out newNgayHieuLuc);
                                                        if (!isValidDate)
                                                        {
                                                            Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại theo định dạng MM/dd/yyyy.");
                                                        }
                                                    } while (!isValidDate);

                                                    Console.WriteLine("Nhập mô tả mới:");
                                                    string newMoTa = Console.ReadLine().Trim();

                                                    Dictionary<string, object> newFieldValues = new Dictionary<string, object>
                                                    {
                                                        {"Tenbanan", newTenBanAn},
                                                        {"Ngayhieuluc", newNgayHieuLuc},
                                                        {"Mota", newMoTa}
                                                    };

                                                    UpdateListItemById(bananList, banAnIdToEdit, newFieldValues); // Thay đổi đối số thành banAnList
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Tên bản án không được để trống. Không thực hiện sửa đổi.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("ID không hợp lệ hoặc không tồn tại!");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("ID không hợp lệ. Vui lòng nhập một số nguyên.");
                                        }

                                        break;
                                    case 3:
                                        Console.WriteLine("Nhập ID của bản án bạn muốn xóa:");
                                        int banAnIdToDelete;
                                        if (int.TryParse(Console.ReadLine(), out banAnIdToDelete))
                                        {
                                            CamlQuery query = CamlQuery.CreateAllItemsQuery();
                                            ListItemCollection items = bananList.GetItems(query);
                                            context.Load(items);
                                            context.ExecuteQuery();
                                            bool isIDfound = false;
                                            foreach (ListItem item in items)
                                            {
                                                if ((int)item["ID"] == banAnIdToDelete)
                                                {
                                                    // Nếu ID tồn tại trong danh sách, đặt isIDfound thành true và thoát khỏi vòng lặp
                                                    isIDfound = true;
                                                    break;
                                                }
                                            }
                                            if (isIDfound)
                                            {
                                                DeleteListItemById(bananList, banAnIdToDelete);
                                            }
                                            else
                                            {
                                                Console.WriteLine("ID không hợp lệ hoặc không tồn tại!");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("ID không hợp lệ. Vui lòng nhập một số nguyên.");
                                        }
                                        break;
                                    case 4:
                                        Console.WriteLine("Danh sách bản án:");
                                        Console.WriteLine("| {0,-9} | {1,-16} | {2,-14} | {3, -12} ","ID_BiCao", "Tên bản án","Ngày hiệu lực","Mô tả");
                                        DisplayBanAnList(bananList);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

        }
        static void DeleteListItemById(List list, int itemId)
        {
            ListItem itemToDelete = list.GetItemById(itemId);
            itemToDelete.DeleteObject();
            list.Context.ExecuteQuery();
            Console.WriteLine($"Mục có ID {itemId} đã được xóa thành công.");
        }

        static Dictionary<int, string> DisplayListItems(List list)
        {
            Dictionary<int, string> itemDict = new Dictionary<int, string>();
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = list.GetItems(query);
            list.Context.Load(items);
            list.Context.ExecuteQuery();

            foreach (ListItem item in items)
            {
                string title = item["Title"].ToString();
                int id = int.Parse(item["ID"].ToString());
                itemDict.Add(id, title);
                Console.WriteLine($"Nhập: {id} - {title}");
            }

            return itemDict;
        }
        static Dictionary<int, string> Hienthibicao(List list)
        {
            Dictionary<int, string> bicaoDict = new Dictionary<int, string>();
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = list.GetItems(query);
            list.Context.Load(items);
            list.Context.ExecuteQuery();

            Console.WriteLine("Danh sách thông tin bị cáo:");
            foreach (ListItem item in items)
            {
                string id = item["ID"].ToString();
                string fullName = item["TenBiCao"].ToString();
                Console.WriteLine($"ID: {id}, Tên bị cáo: {fullName}");
                bicaoDict.Add(int.Parse(id), fullName);
            }

            return bicaoDict;
        }

        static void UpdateListItemById(List list, int itemId, Dictionary<string, object> fieldValues)
        {
            ListItem itemToUpdate = list.GetItemById(itemId);
            foreach (var fieldValue in fieldValues)
            {
                itemToUpdate[fieldValue.Key] = fieldValue.Value;
            }
            itemToUpdate.Update();
            list.Context.ExecuteQuery();
            Console.WriteLine($"Mục có ID {itemId} đã được cập nhật thành công.");
        }
        static void DisplayBicaoList(List list, int pageIndex, int totalPages, int pageSize)
        {
            
            int startItemIndex = (pageIndex - 1) * pageSize;
            if (pageIndex > totalPages)
            {
                Console.WriteLine("Lỗi: Số trang vượt quá tổng số trang.");
                return;
            }

            // Tạo truy vấn CAML để lấy mục với phân trang
            CamlQuery camlQuery = new CamlQuery();
            
            camlQuery.ViewXml = $@"<View>
                                <RowLimit>{pageSize * (pageIndex - 1)}</RowLimit>
                                <Query>
                                    <OrderBy>
                                        <FieldRef Name='ID' Ascending='TRUE'/>
                                    </OrderBy>
                                    <ViewFields>
                                        <FieldRef Name='ID'/>
                                    </ViewFields>
                                </Query>
                            </View>";

            ListItemCollectionPosition position = null;
            camlQuery.ListItemCollectionPosition = position;
            ListItemCollection items = list.GetItems(camlQuery);
            list.Context.Load(items);
            list.Context.ExecuteQuery();

            position = items.ListItemCollectionPosition;
            CamlQuery camlQuery1 = new CamlQuery();

            camlQuery1.ViewXml = $@"<View>
                                <RowLimit>{pageSize}</RowLimit>
                                <Query>
                                    <OrderBy>
                                        <FieldRef Name='ID' Ascending='TRUE'/>
                                    </OrderBy>
                                </Query>
                            </View>";

            camlQuery1.ListItemCollectionPosition = position;
            items = list.GetItems(camlQuery1);
            list.Context.Load(items);
            list.Context.ExecuteQuery();
            // Hiển thị dữ liệu của trang hiện tại
            foreach (ListItem item in items)
            {
                string id = item["ID"].ToString();
                string fullName = item["TenBiCao"].ToString();
                string otherName = item["TenKhac"].ToString();
                DateTime birthDate = (DateTime)item["NgaySinh"];
                string quocTich = GetLookupFieldValue(item, "QuocTich");
                string diaChi = GetLookupFieldValue(item, "DiaChi");
                string fatherName = item["HoTenCha"].ToString();
                string motherName = item["HoTenMe"].ToString();
                Console.WriteLine("| {0,-3} | {1,-16} | {2,-12} | {3,-12} | {4,-10} | {5,-12} | {6,-16} | {7,-16} |", id, fullName, otherName, birthDate.ToString("MM/dd/yyyy"), quocTich, diaChi, fatherName, motherName);
            }

            // Hiển thị thông tin phân trang
            Console.WriteLine($"Trang {pageIndex}");
            Console.WriteLine();

        }
        static void PhanTrang(List list)
        {
            int pageSize = 3;
            int totalItems = list.ItemCount; 
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            int pageIndex = 1;
            DisplayBicaoList(list, pageIndex, totalPages, pageSize );
            while (true)
            {
                Console.WriteLine("Nhấn 'N' để chuyển sang trang tiếp theo, 'P' để quay lại trang trước, hoặc 'Q' để thoát:");
                char key = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (key)
                {
                    case 'n':
                    case 'N':
                        pageIndex++;
                        DisplayBicaoList(list, pageIndex, totalPages, pageSize);
                        break;
                    case 'p':
                    case 'P':
                        if (pageIndex > 1)
                        {
                            pageIndex--;
                            DisplayBicaoList(list, pageIndex, totalPages, pageSize);
                        }
                        else
                        {
                            Console.WriteLine("Bạn đang ở trang đầu tiên!");
                        }
                        break;
                    case 'q':
                    case 'Q':
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }
        static void DisplayBanAnList(List list)
        {
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = list.GetItems(query);
            list.Context.Load(items);
            list.Context.ExecuteQuery();

            foreach (ListItem item in items)
            {
                // var lookupFieldValue = (FieldLookupValue)item["ID_Bicao"];
                string id = GetLookupFieldValue(item, "ID_Bicao");
                string tenBanAn = item["TenBanAn"].ToString();
                string ngayHieuLuc = ((DateTime)item["NgayHieuLuc"]).ToString("MM/dd/yyyy");
                string moTa = item["MoTa"].ToString();
                Console.WriteLine("| {0,-9} | {1,-16} | {2,-14} | {3, -12} ",id,tenBanAn,ngayHieuLuc,moTa);
            }
        }

        static string GetLookupFieldValue(ListItem item, string fieldName)
        {
            FieldLookupValue fieldValue = item[fieldName] as FieldLookupValue;
            return fieldValue != null ? fieldValue.LookupValue : "";
        }

    }
}
