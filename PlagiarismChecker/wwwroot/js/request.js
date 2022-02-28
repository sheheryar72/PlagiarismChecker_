var selectedFiles = []
window.addEventListener('load', function () {

    $(document).on('change', '#select-files', function () {
        console.log("asd");
        var fileData = new FormData();
        for (eachFile of $(this)[0].files) {
            fileData.append('file', eachFile);
        }
        console.log(fileData);

        const url = '/home/GetPlagiarism';

        let request = new Request(url, {
            method: 'POST',
            body: fileData,
        });

        fetch(request)
            .then(res => res.json())
            .then((res) => {
                console.log(res);
                PopulateResultOnTable(res);
            });

    });


    function PopulateResultOnTable(res) {
        var tableBodyHtml = '';
        for (let i = 0; i < res.length; i++) {
            tableBodyHtml += `<tr>
                                <td>${i + 1}</td>
                                <td>${res[i].firstDoc}</td>
                                <td>${res[i].secondDoc}</td>
                                <td>${parseFloat(res[i].score * 100).toFixed(3)} %</td>
                            </tr>`
		}
        var tableHtml = ` <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Source File</th>
                                    <th>Compare With</th>
                                    <th>Plagiarism</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${tableBodyHtml}
                            </tbody>
                        </table>`;
        $('#table-score').html(tableHtml);
	}

});