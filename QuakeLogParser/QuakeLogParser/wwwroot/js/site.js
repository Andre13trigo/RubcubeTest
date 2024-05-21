function showData() {
    $.ajax({
        url: 'https://localhost:44375/api/Dados/GetDados',
        type: 'POST',
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                var html = ''
            }
        }
    });
}
