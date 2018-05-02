using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPL.Models
{
    public class GioHang
    {
        DBNPLDataContext data = new DBNPLDataContext();        
        public string sHinhAnh { set; get; }
        public double dPhiVanChuyen { set; get; }
        public int iIDThucDon { set; get; }
        public int iIDMonAn { set; get; }
        public string sTenThucDon { set; get; }
        public double dDonGia { set; get; }
        public int iSoLuong { set; get; }
       public int iID { set; get; }
        public double dThanhTien 
        {
            get { return (dDonGia * iSoLuong) + dPhiVanChuyen; }
        }
        //khoi tao gio hang theo ma thuc don duoc truyen vao
        public GioHang(int IDThucDon)
        {
            iIDThucDon = IDThucDon;        
            ThucDon td = data.ThucDons.Single(n => n.IDThucDon == iIDThucDon);
            dPhiVanChuyen = double.Parse( td.MonAn.PhiVanChuyen.ToString());      
            sTenThucDon = td.TenThucDon;
            sHinhAnh = td.HinhAnh;            
            dDonGia = double.Parse(td.DonGia.ToString());
            iSoLuong = 1;                      
        }       
    }
}