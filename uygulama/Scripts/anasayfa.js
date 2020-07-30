
$("#cikisbuton").click(function (e) {
    e.preventDefault();
    $.ajax({
        url: '/Home/CikisYap',
        type: 'POST',
        data: {},
        success: function (msg) {
            
            window.location.href = "/Home/GirisYap";
        },
        error: function (hata, thrownError) {
            alert(hata.status);
            alert(thrownError);
            alert(hata.responseText);
        }
    });
});

