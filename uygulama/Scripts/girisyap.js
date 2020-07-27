
$(".buton").click(function (e) {
    e.preventDefault();
    $.ajax({
        url: '/Home/GirisYap',
        type: 'POST',
        data: {
            KullaniciAdi: $('#exampleInputKullaniciAdi').val(),
            Sifre: $('#exampleInputPassword1').val(),
            BeniHatirla: $('input[name="radiobuton"]:checked').val()
        },
        success: function (data) {
            if (data.isactivate) {
                $(data).each(function (index, value) {
                    console.log(value);
                });
                window.location.href = "/Home/Anasayfa";
                
            } else {
                $(data).each(function (index, value) {
                    console.log(value);
                });
                window.location.href = "/Uye/UyeAktivasyonu?id=" + data.ActivateGuid;    
            }
           
        },
        error: function (hata, thrownError) {
            alert(hata.status);
            alert(thrownError);
            alert(hata.responseText);
        }
    });

});

//function GirisYap() {
//    window.location.href = $(this).data('url');
//}