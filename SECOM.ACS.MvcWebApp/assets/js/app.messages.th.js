if (app.messages)
{
    $.extend(true, app.messages, {
        ajax: {
            "loading": "กรุณารอสักครู่ กำลังร้องขอข้อมูล.."
        },
        validators: {
            "required": "กรุณาระบุ{0}"
        },
        confirmation: {
            "proceedButtonText": "ดำเนินการ",
            "cancelButtonText": "ยกเลิก"
        },
        dataSource: {
            "requestEnd": {
                "readNoRecord": "ไม่พบข้อมูลจากการร้องขอ.",
                "updateSuccess": "ข้อมูลถูกบันทึกเรียบร้อยแล้ว.",
                "createSuccess": "ข้อมูลถูกเพิ่มเรียบร้อยแล้ว.",
                "destroySuccess": "ข้อมูลถูกลบเรียบร้อยแล้ว."
            },
            "noRecordFound": "ไม่พบข้อมูลจากการค้นหา.",
        },
        grid: {
            popupEditor: {
                create: {
                    title: 'เพิ่มข้อมูล',
                    button: { text: "บันทึก" }
                },
                edit: {
                    title: 'แก้ไขข้อมููล',
                    button: { text: "บันทึก" }
                },
                cancelButton: {
                    text: "ยกเลิก"
                }
            }
        },
        upload: {
            cancel: "ยกเลิก",
            clearSelectedFiles: "ล้างไฟล์ที่เลือก",
            remove: "ลบ",
            select: "เลือก"
        },
        scrollToTop: "เลื่อนไปข้างบน"
    });
}

