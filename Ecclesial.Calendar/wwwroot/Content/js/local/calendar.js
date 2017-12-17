var currentUser = {};
var isSaving = false;
var handled = false;
const calendarId = window.location.pathname.split('/').slice(-1).pop();

const t_schedule = Handlebars.compile(document.getElementById('t-schedule').innerHTML);
const t_task_tags = Handlebars.compile(document.getElementById('t-task-tags').innerHTML);

const formatParticipantName = function (participant) {
    if (participant && participant.firstName)
        return participant.firstName.substring(0, 2) + ' ' + participant.lastName;
    return '';
};

$(document).ready(function () {
    const nav = priorityNav.init();
    const tab = $('#tab-schedule').tabs();

    // Fetch data.
    var fetch = Promise.all([
        $.ajax('/data/participant', {
            method: 'GET',
            dataType: 'json'
        })
    ]).then(function (data) {
        currentUser = data[0];
        }).then(function () {
            // Sunday
            $.ajax('/data/schedule/' + calendarId, {
                method: 'GET',
                dataType: 'json',
                success: function (data, status, xhr) {
                    drawSundaySchedule(data);
                }
            })

            // Bible class
            $.ajax('/data/schedule/' + calendarId + '/4', {
                method: 'GET',
                dataType: 'json',
                success: function (data, status, xhr) {
                    drawBibleClassSchedule(data);
                }
            });

            // Cleaning
            $.ajax('/data/schedule/' + calendarId + '/6', {
                method: 'GET',
                dataType: 'json',
                success: function (data, status, xhr) {
                    drawCleaningSchedule(data);
                }
            });
        }).catch(function (error) {
            alert(error);
        });

    const handleTouch = function (e, handle) {
        e.stopImmediatePropagation();

        if (e.type === 'touched') {
            handled = true;
            handle.call(this, e);
        } else if (e.type === 'click' && !handled) {
            handle.call(this, e);
        } else {
            handled = false;
        }
    };

    const acceptTaskFn = function (e) {
        var $btn = $(this);
        const taskId = $btn.data('task-id');
        $.ajax('/data/schedule/accept/task/' + taskId, {
            method: 'POST',
            dataType: 'json',
            success: function (data, status, xhr) {
                var $btn2 = $("<button>", { 'name': 'decline', 'data-task-id': taskId }).button();
                $btn2.text(formatParticipantName(data.participant));
                $btn2.on('touchend click', declineTask);
                var $tags = $btn.parent().next('td[name=\'tags\']');
                if ($tags.length === 1) {
                    var $new = $(t_task_tags(data.task));
                    $new.children('input[name=\'task-tag\']').on('input', saveTag);
                    $tags.replaceWith($new);
                }
                $btn.replaceWith($btn2);
            }
        });
    };

    const acceptTask = function (e) {
        handleTouch.call(this, e, acceptTaskFn);
    };

    const declineTaskFn = function (e) {
        var $btn = $(this);
        const taskId = $btn.data('task-id');
        $.ajax('/data/schedule/decline/task/' + taskId, {
            method: 'DELETE',
            dataType: 'json',
            success: function (data, status, xhr) {
                var $btn2 = $("<button>", { 'name': 'accept', 'data-task-id': taskId }).button();
                $btn2.text("select");
                $btn2.on('touchend click', acceptTask);
                var $tags = $btn.parent().next('td[name=\'tags\']');
                if ($tags.length === 1) {
                    var $new = $(t_task_tags(data.task));
                    $tags.replaceWith($new);
                }
                $btn.replaceWith($btn2);
            }
        });
    };

    const declineTask = function (e) {
        handleTouch.call(this, e, declineTaskFn);
    };

    const saveTag = function (e) {
        e.stopImmediatePropagation();
        if (!isSaving) {
            isSaving = true;
            var $input = $(this);
            const taskId = $input.data('task-id');
            const tagId = $input.data('tag-id');
            const rowVersion = $input.data('tag-rv');
            const tagValue = $input.val();
            $.ajax('/data/schedule/tag', {
                method: 'PUT',
                dataType: 'json',
                data: { taskId: taskId, id: tagId, value: tagValue, rowVersion: rowVersion },
                success: function (data, status, xhr) {
                    $input.data('tag-rv', data.rowVersion);
                    isSaving = false;
                }
            });
        }
    };

    const drawSundaySchedule = function (data) {
        var $cal = $('#sundaySchedule');
        $cal.html(t_schedule(data));

        $cal.find('button[name=\'task-accept\']').button().on('touchend click', acceptTask );
        $cal.find('button[name=\'task-decline\']').button().on('touchend click', declineTask );
        $cal.find('input[name=\'task-tag\']').on('input', saveTag );
    };

    const drawBibleClassSchedule = function (data) {
        var $cal = $('#bibleClassSchedule');
        $cal.html(t_schedule(data));

        $cal.find('button[name=\'task-accept\']').button().on('touchend click', acceptTask);
        $cal.find('button[name=\'task-decline\']').button().on('touchend click', declineTask);
        $cal.find('input[name=\'task-tag\']').on('input', saveTag);
    };

    const drawCleaningSchedule = function (data) {
        var $cal = $('#cleaningSchedule');
        $cal.html(t_schedule(data));

        $cal.find('button[name=\'task-accept\']').button().on('touchend click', acceptTask);
        $cal.find('button[name=\'task-decline\']').button().on('touchend click', declineTask);
        $cal.find('input[name=\'task-tag\']').on('input', saveTag);
    };
});

Handlebars.registerPartial('taskTags', t_task_tags);

Handlebars.registerPartial('participantName', function (participant, options) {
    return formatParticipantName(participant);
});

Handlebars.registerHelper('isParticipant', function (participant, options) {
    if (Array.isArray(participant)) {
        var isParticipant = false;
        participant.some(function (p) {
            if (p.id === currentUser.id) {
                isParticipant = true;
                return p;
            }
        });
        if (isParticipant)
            return options.fn(this);
    } else if ('number' === typeof participant && participant == currentUser.id) {
        return options.fn(this);
    }
    return options.inverse(this);
});

Handlebars.registerHelper('isAllowed', function (task, options) {
    var isAllowed = true;
    task.attributes.some(function (ta) {
        if (ta.type === 1) {
            // The user must have the attribute.
            if (!currentUser.attributes.find(function (ua) {
                return ua.key === ta.key && ua.value === ta.value;
            })) {
                isAllowed = false;
                return ta;
            }
        } else if (ta.type === 2) {
            // The user cannot have the attribute.
            if (currentUser.attributes.find(function (ua) {
                return ua.key === ta.key && ua.value === ta.value;
            })) {
                isAllowed = false;
                return ta;
            }
        }
    });
    if (isAllowed)
        return options.fn(this);
    else {
        return options.inverse(this);
    }
});

Handlebars.registerHelper('multiParticipants', function (task, options) {
    if ('undefined' !== typeof task) {
        var result = '';
        for (let i = 0; i < task.maxParticipants - task.participants.length; i++) {
            result += options.fn(i);
        }
        return result;
    }

});