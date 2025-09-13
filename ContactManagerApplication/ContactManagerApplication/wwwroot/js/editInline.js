$(document).ready(function () {
    console.log("editInline.js is loaded and ready!");

    $(document).on('click', '.edit-btn', function () {
        console.log("Button 'Edit' clicked!");

        var row = $(this).closest('tr');
        console.log("Row data for editing:", row);

        row.find('.editable-name').html('<input type="text" class="form-control" value="' + row.find('.editable-name').text().trim() + '">');
        row.find('.editable-date').html('<input type="date" class="form-control" value="' + row.find('.editable-date').text().trim() + '">');
        row.find('.editable-married').html('<input type="checkbox" ' + (row.find('.editable-married').text().trim() === 'True' ? 'checked' : '') + ' class="form-check-input">');
        row.find('.editable-phone').html('<input type="text" class="form-control" value="' + row.find('.editable-phone').text().trim() + '">');
        row.find('.editable-salary').html('<input type="text" class="form-control" value="' + row.find('.editable-salary').text().trim() + '">');

        row.find('.edit-btn').addClass('d-none');
        row.find('.save-btn, .cancel-btn').removeClass('d-none');

        console.log("Row switched to edit mode!");
    });

    $(document).on('click', '.save-btn', function () {
        console.log("Button 'Save' clicked!");

        var row = $(this).closest('tr');
        var id = row.data('id');

        console.log("Saving row data for ID:", id);

        var editedData = {
            Id: id,
            Name: row.find('.editable-name input').val(),
            DateOfBirth: row.find('.editable-date input').val(),
            Married: row.find('.editable-married input').is(':checked'),
            Phone: row.find('.editable-phone input').val(),
            Salary: parseFloat(row.find('.editable-salary input').val().replace(/\s/g, '').replace(',', '.'))
        };

        if (!editedData.Name.trim()) {
            alert("Name cannot be empty.");
            return;
        }
        if (!editedData.Phone.trim()) {
            alert("Name cannot be empty.");
            return;
        }
        if (new Date(editedData.DateOfBirth) > new Date()) {
            alert("Date of birth cannot be in the future.");
            return;
        }
        if (editedData.Salary <= 0 || isNaN(editedData.Salary)) {
            alert("Salary must be a valid positive number.");
            return;
        }

        console.log("Edited data to send:", editedData);

        $.ajax({
            url: '/Contact/EditInline',
            type: 'POST',
            data: editedData,
            success: function (response) {
                console.log("Response from server:", response);
                //alert('Data updated successfully!');
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Error while saving data:", error);
                alert('Failed to update data.');
            }
        });
    });

    $(document).on('click', '.cancel-btn', function () {
        console.log("Button 'Cancel' clicked!");
        location.reload();
    });
});