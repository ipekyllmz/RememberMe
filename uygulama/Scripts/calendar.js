
// Setup the calendar with the current date


$(document).ready(function () {
    var date = new Date();
    var month = date.getMonth();
    var months = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var day = date.getDate().toString();
       
    // Set click handlers for DOM elements
    $(".right-button").click({ date: date }, next_year);
    $(".left-button").click({ date: date }, prev_year);
    $(".month").click({ date: date }, month_click);
    $("#add-button").click({ date: date }, new_event);
    $("#cancel-button").click({ date: date });
    // Set current month as active
    $(".months-row").children().eq(date.getMonth()).addClass("active-month");
    init_calendar(date);
    var events = check_events(day, date.getMonth() + 1, date.getFullYear());
    show_events(events, months[date.getMonth()], day);
    
});

// Initialize the calendar by appending the HTML dates
function init_calendar(date) {
    $(".tbody").empty();
    $(".events-container").empty();
    var calendar_days = $(".tbody");
    var month = date.getMonth();
    var year = date.getFullYear();
    var day_Not = days_in_month(month, year);
    var row = $("<tr class='table-row'></tr>");
    var today = date.getDate();
    // Set date to 1 to find the first day of the month
    date.setDate(1);
    var first_day = date.getDay();
    // 35+firstDay is the number of date elements to be added to the dates table
    // 35 is from (7 days in a week) * (up to 5 rows of dates in a month)
    for (var i = 0; i < 35 + first_day; i++) {
        // Since some of the elements will be blank, 
        // need to calculate actual date from index
        var day = i - first_day + 1;
        // If it is a sunday, make a new row
        if (i % 7 === 0) {
            calendar_days.append(row);
            row = $("<tr class='table-row'></tr>");
        }
        // if current index isn't a day in this month, make it blank
        if (i < first_day || day > day_Not) {
            var curr_date = $("<td class='table-date nil'>" + "</td>");
            row.append(curr_date);
        }
        else {
            var curr_date = $("<td class='table-date'>" + day + "</td>");
            var events = check_events(day, month + 1, year);
            if (today === day && $(".active-date").length === 0) {
                curr_date.addClass("active-date");
                show_events(events, months[month], today);
            }
            // If this date has any events, style it with .event-date
            if (events.length !== 0) {
                curr_date.addClass("event-date");
            }
            // Set onClick handler for clicking a date
            curr_date.click({ events: events, month: months[month], day: day }, date_click);
            row.append(curr_date);
        }
    }
    // Append the last row and set the current year
    calendar_days.append(row);
    $(".year").text(year);
}

// Get the number of days in a given month/year
function days_in_month(month, year) {
    var monthStart = new Date(year, month, 1);
    var monthEnd = new Date(year, month + 1, 1);
    return (monthEnd - monthStart) / (1000 * 60 * 60 * 24);
}

// Event handler for when a date is clicked
function date_click(event) {
    var day = $(".active-date").html();
    var month = $(".active-month").html();
    var yıl = $("#label").html();
    var tarih = day + month + yıl;
    $(".events-container").show(250);
    $("#dialog").hide(250);
    $(".active-date").removeClass("active-date");
    $(this).addClass("active-date");
    $(".active-date").click(function (event) {


        $.ajax({
            url: '/Home/EtkinlikGetir',
            type: 'POST',
            async: false,
            data: { date: tarih },
            success: function (data) {

                $(data).each(function (value) {
                    console.log(value);
                    event_data.events.push(data);
                    show_events(event_data.events, month, day);
                });


            },
            error: function (hata, thrownError) {
                alert(hata.status);
                alert(thrownError);
                alert(hata.responseText);
            }
        });
        });
    show_events(event.data.events, event.data.month, event.data.day);
    };

// Event handler for when a month is clicked
function month_click(event) {
    $(".events-container").show(250);
    $("#dialog").hide(250);
    var date = event.data.date;
    $(".active-month").removeClass("active-month");
    $(this).addClass("active-month");
    var new_month = $(".month").index(this);
    date.setMonth(new_month);
    init_calendar(date);
}

// Event handler for when the year right-button is clicked
function next_year(event) {
    $("#dialog").hide(250);
    var date = event.data.date;
    var new_year = date.getFullYear() + 1;
    $("year").html(new_year);
    date.setFullYear(new_year);
    init_calendar(date);
}

// Event handler for when the year left-button is clicked
function prev_year(event) {
    $("#dialog").hide(250);
    var date = event.data.date;
    var new_year = date.getFullYear() - 1;
    $("year").html(new_year);
    date.setFullYear(new_year);
    init_calendar(date);
}

// Event handler for clicking the new event button
function new_event(event) {
    // if a date isn't selected then do nothing
    if ($(".active-date").length === 0)
        return;
   
    // remove red error input on click
    $("input").click(function () {
        $(this).removeClass("error-input");
    })
    // empty inputs and hide events
    $("#dialog input[type=text]").val('');
    $("#dialog input[type=text]").val('');
    $("#dialog input[type=text]").val('');
    $(".events-container").hide(250);
    $("#dialog").show(250);
    // Event handler for cancel button
    $("#cancel-button").click(function () {
        $("#EtkinlikAdi").removeClass("error-input");
        $("#Not").removeClass("error-input");
        $("#Kisi").removeClass("error-input");
        $("#dialog").hide(250);

        $(".events-container").show(250);
    });
    // Event handler for ok button
    $("#ok-button").click({ date: event.data.date }, function (event) {
        var date = event.data.date;
        var day = $(".active-date").html();
        var month = $(".active-month").html();
        var yıl = $("#label").html();
        var EtkinlikAdi = $("#EtkinlikAdi").val();
        var tarih = day + month + yıl;
        var Not = ($("#Not").val());
        //var day = parseInt($(".active-date").html());
        var Kisi = $("#Kisi").val();
        // Basic form validation
        if (EtkinlikAdi.length === 0) {
            $("#EtkinlikAdi").addClass("error-input");
        }
        else  {
            $("#dialog").hide(250);
            console.log("new event");
            new_event_json(EtkinlikAdi, Not, date, day, Kisi);
            date.setDate(day);
            init_calendar(date);
        }
        
        $.ajax({
            url: '/Home/EtkinlikEkle',
            type: 'POST',
            dataType: 'json',
            data: {
                'EtkinlikAdi': EtkinlikAdi,
                'Not': Not,
                'DogumTarihi': tarih,
                'DogumGunu': day,
                'Kisi': Kisi
            },
            success: function (data) {
            alert(data.d);
        }
        })
    });
  
}

// Adds a json event to event_data
function new_event_json(EtkinlikAdi, Not, date, day, Kisi) {
  
    var event = {
        "Etkinlik": EtkinlikAdi,
        "Kisi": Kisi,
        "Not": Not,
        "year": date.getFullYear(),
        "month": date.getMonth() + 1,
        "day": day

        
    };
    event_data["events"].push(event);
}

// Display all events of the selected date in card views
function show_events(events, month, day) {
   
    // Clear the dates container
    $(".events-container").empty();
    $(".events-container").show(250);
    console.log(event_data["events"]);
    // If there are no events for this date, notify the user
    if (events.length === 0) {
        var event_card = $("<div class='event-card'></div>");
        var event_EtkinlikAdi = $("<div class='event-EtkinlikAdi'>" + day + " " + month + " için planlanmış hiç etkinlik yok.</div>");
        $(event_card).css({ "border-left": "10px solid #FF1744" });
        $(event_card).append(event_EtkinlikAdi);
        $(".events-container").append(event_card);
    }
    else
    {
        // Go through and add each event as a card to the events container
        for (var i = 0; i < events.length; i++) {
           
                var event_card = $("<div class='event-card'></div>");
                var event_EtkinlikAdi = $("<div class='event-EtkinlikAdi'>Etkinlik Adı:" + events[i]["EtkinlikAdi"] + "</div>");
                var event_Kisi = $("<div class='event-Kisi'>Kişi:" + events[i]["Kisi"] + " </div>");
                var event_Not = $("<div class='event-Not'>Not:" + events[i]["Not"] + " </div>");
          
            
           
            //if (events[i]["cancelled"] === true) {
            //    $(event_card).css({
            //        "border-left": "10px solid #FF1744"
            //    });
            //    event_Not = $("<div class='event-cancelled'>İptal Edildi.</div>");
            //}
            $(event_card).append(event_EtkinlikAdi).append(event_Not).append(event_Kisi);
            $(".events-container").append(event_card);
        }
    }
}

// Checks if a specific date has any events
function check_events(day, month, year,) {
    var events = [];
    for (var i = 0; i < event_data["events"].length; i++) {
        var event = event_data["events"][i];
        if (event["day"] === day &&
            event["month"] === month &&
            event["year"] === year) {
            events.push(event);
        }
    }
    return events;
}

// Given data for events in JSON format
var event_data = {
    "events": [
        {
            "EtkinlikAdi": "parti1 ",
            "Not": "selam",
            "Kisi":"ipek",
            "year": 2020,
            "month": 7,
            "day": 25,
            "cancelled": true
        },
        {
            "EtkinlikAdi": "parti2 ",
            "Not": "naber",
            "Kisi": "özlem",
            "year": 2020,
            "month": 7,
            "day": 8
        }
    ]
};

const months = [
    "Ocak",
    "Şubat",
    "Mart",
    "Nisan",
    "Mayıs",
    "Haziran",
    "Temmuz",
    "Ağustos",
    "Eylül",
    "Ekim",
    "Kasım",
    "Aralık"
];