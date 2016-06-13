﻿$(function () {

    //================================================================================
    // Setup the auto-generated proxy for the hub.
    //================================================================================
    $.connection.hub.url = "/signalr";

    var chat = $.connection.mIMHub;
    var client = chat.client;
    var server = chat.server;

    //================================================================================
    // Global Variables for playerCreation
    //================================================================================
    var race = "Human";
    var playerClass;
    var str;
    var dex;
    var con;
    var int;
    var wis;
    var cha;
    var gender;
    var name;
    var email;
    var password;

    //================================================================================
    // Helper Functions
    //================================================================================
    var MIM = {
        getRaceInfo: function (getValue) {
            server.characterSetupWizard(getValue, "race");
        },
        getClassInfo: function (getValue) {
            server.characterSetupWizard(getValue, "class");
        },
        selectOption: function () {
            $(".modal-body a").click(function () {

                if ($(this).hasClass("active")) {

                    //don't call server if same tab
                    return false;
                }
                else {
                    $(".modal-body a").removeClass("active");
                    $(this).addClass("active");

                    var getValue = $(this).text().trim().toLowerCase().toString();

                    if ($('#select-race').is(':visible')) {
                        MIM.getRaceInfo(getValue)
                    }
                    else if ($('#select-class').is(':visible')) {
                        MIM.getClassInfo(getValue)
                    }

                   
                }
            });

        },
        CharacterNextStep: function () {
            var raceStep = document.getElementById('select-race');
            var classStep = document.getElementById('select-class');
            var statsStep = document.getElementById('select-stats');
            var infoStep = document.getElementById('select-char');
            var modelHeaderDiv = $(".modal-header div");
            var classBreadCrumb = $(".classBreadCrumb");

            $("#RaceSelectedBtn").click(function () {
                MIM.getClassInfo("fighter");
                raceStep.style.display = "none";
                classStep.style.display = "block";
                modelHeaderDiv.removeClass("active");
                classBreadCrumb.addClass("active");
            });

            $("#backToRace").click(function () {         
                raceStep.style.display = "block";
                classStep.style.display = "none";
                modelHeaderDiv.removeClass("active");
                $(".raceBreadCrumb").addClass("active");
            });


            $("#selectedClassBtn").click(function () {
                server.getStats();
                classStep.style.display = "none";
                statsStep.style.display = "block";
                modelHeaderDiv.removeClass("active");
                $(".statsBreadCrumb").addClass("active");
            });

            $("#backToClass").click(function () {
                 
                classStep.style.display = "block";
                statsStep.style.display = "none";
                modelHeaderDiv.removeClass("active");
                classBreadCrumb.addClass("active");
            });

            $("#reRollStats").click(function() {
                server.getStats();
            });

            $("#selectedStatsBtn").click(function () {
               // server.getStats();
                statsStep.style.display = "none";
                infoStep.style.display = "block";
                modelHeaderDiv.removeClass("active");
                $(".infoBreadCrumb").addClass("active");
            });


            $("#CreateCharBtn").click(function () {

               
                name = document.getElementById('playerName').value.trim();
                gender = $("input[name=gender]:checked").val();
                email = document.getElementById('email').value.trim();
                
                var pass = document.getElementById('password').value;
                var confirmPass = document.getElementById('confirmPassword').value;

                if (pass == confirmPass && confirmPass != "") {
                    password = confirmPass;
                }
                else {
                    document.getElementById('passwordMatchError').style.display = "block";
                    document.getElementById('confirmPassword').className += " has-error";
                    alert("error")
                    return false;
                }

                alert("create char")

            });
        },
        sendMessageToServer: function () {
            $('#sendmessage').keypress(function (e) {
                var key = e.which;
                if (key == 13)  // the enter key code
                {
                    var message = $('#message');
                    var playerGuid = $.connection.hub.id;

                    // Call the Send method on the hub.
                    server.recieveFromClient(message.val(), playerGuid);

                    message.select().focus();
                    return false;
                }
            });

            $('#sendmessage').click(function () {

                var message = $('#message');
                var playerGuid = $.connection.hub.id;

                // Call the Send method on the hub.
                server.recieveFromClient(message.val(), playerGuid);

                message.select().focus();
            });
        },
        htmlEncode: function (value) {
            var encodedValue = $('<div />').text(value).html();
            return encodedValue;
        },
        createCharacter: function(char) {

           // alert(char.name);
            server.welcome();
           
            server.charSetup($.connection.hub.id, char.Name, char.Email, char.Password, char.Gender, char.Race, char.Class, char.Strength, char.Dexterity, char.Constitution, char.Wisdom, char.Intelligence, char.Charisma);
          //  server.loadRoom($.connection.hub.id);

            document.getElementById('signUpModal').style.display = "none";

           


        },
        login: function (char) {
 
            server.login($.connection.hub.id, char.Name, char.password);

            document.getElementById('signUpModal').style.display = "none";

          //server.getChar($.connection.hub.id, char.Name);
          
        },
        getGuid: function (guid) {
            var guid = $.cookie("playerGuid");
            return guid;
        },
        UI: {
            setWindowHeight: function() {
                var viewPort = $(window).height() - 60;
                $("#discussion").css({"height": viewPort, "max-height": viewPort});
            }
        },
        init: function () {
            console.log("INIT")
            //init when signalr is ready
            MIM.selectOption();
            MIM.CharacterNextStep();
            MIM.UI.setWindowHeight();

            var resizeTimer;

            $(window).on('resize', function (e) {

                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {

                    MIM.UI.setWindowHeight();

                }, 250);

            });
        }
    }




    //// Set focus to input box ////
    $('#message').focus();

    //================================================================================
    // Client Functions
    //================================================================================

  
    client.quit = function () {
       
        $.connection.hub.stop();
    };
    //// Add a new message to the page ////
    client.addNewMessageToPage = function (message) {
        $('#discussion').append("<p>" + message + "</p>");
    };

    //// Add a new message to the page ////
    client.updateScore = function (score) {
     console.log("score: " + score)
        var playerData = score;

     
        $('#player-name').html(score.Name);
        $('#player-level').html(score.Level);
        $('#player-race').html(playerData.Race);
        $('#player-class').html(playerData.SelectedClass);
        $('#player-gender').html(playerData.Gender);
        $('#player-alignment').html(playerData.AlignmentScore);

        $('#player-str').html(score.Strength);
        $('#player-max-str').html(score.Strength);
        $('#player-dex').html(score.Dexterity);
        $('#player-max-dex').html(score.Dexterity);
        $('#player-con').html(score.Constitution);
        $('#player-max-con').html(score.Constitution);
        $('#player-wis').html(score.Wisdom);
        $('#player-max-wis').html(score.Wisdom);
        $('#player-int').html(score.Intelligence);
        $('#player-max-int').html(score.Intelligence);
        $('#player-cha').html(score.Charisma);
        $('#player-max-cha').html(score.Charisma);

        $('#player-hp').html(score.HitPoints);
        $('#player-max-hp').html(score.MaxHitPoints);

        $('#player-mana').html(score.ManaPoints);
        $('#player-max-mana').html(score.MaxManaPoints);

        $('#player-end').html(score.MovePoints);
        $('#player-max-end').html(score.MaxMovePoints);

        $('#player-hitroll').html(score.HitRoll);
        $('#player-damroll').html(score.DamRoll);
        $('#player-wimpy').html(score.Wimpy);

        $('#player-weight').html(score.Weight);
        $('#player-max-weight').html(score.MaxWeight);
        $('#player-status').html(score.Status);

        $('#player-hours').html(score.Hours);
        $('#player-experience').html(score.Experience);

        $('#player-copper').html(score.Copper);
        $('#player-silver').html(score.Silver);
        $('#player-gold').html(score.Gold);


    };

    //// Update Race Info ////
    client.updateCharacterSetupWizard = function (step, dataName, dataHelp, dataImg) {

        var info = "<h2>" + dataName + "</h2>" + "<p>" + dataHelp + "</p>";

        if (step === "race") {
            race = dataName; //global Race

            $('.raceInfo').html(info);

            $('.raceImg').attr('src', dataImg);

            $('#Race').val(dataName);
        }
        else if (step === "class") {
            playerClass = dataName; //global player Class
            $('.classInfo').html(info);

            $('.classImg').attr('src', dataImg);

            $('#Class').val(dataName);
        }
      

        console.log(dataName + " " + dataHelp )

     

    }

    //// generate Stats ////
    client.setStats = function (stats) {

        str = stats[0];
        dex = stats[1];
        con = stats[2];
        int = stats[3];
        wis = stats[4];
        cha = stats[5];

        $('#statStr').html(str);
        $('#statDex').html(dex);
        $('#statCon').html(con);
        $('#statInt').html(int);
        $('#statWis').html(wis);
        $('#statCha').html(cha);

        $("#Strength").val(str);
        $("#Dexterity").val(dex);
        $("#Constitution").val(con);
        $("#Intelligence").val(int);
        $("#Wisdom").val(wis);
        $("#Charisma").val(cha);

    }

    client.savePlayerGuid = function (guid) {
        $.cookie("playerGuid", guid);
    }



    //================================================================================
    // Hub has loaded & Server functions
    //================================================================================
    $.connection.hub.start().done(function () {

        //// Load 1st race choice
         MIM.getRaceInfo("human"); //set default;

        /// send info to server
        MIM.sendMessageToServer();

        //// Start scripts
        MIM.init();

        $("#createCharaterForm").on("submit", function (event) {
            var $this = $(this);
            var frmValues = $this.serialize();
            $.ajax({
                type: $this.attr('method'),
                url: $this.attr('action'),
                data: frmValues
            })
            .done(function (data) {

                    MIM.createCharacter(data);

                })
            .fail(function () {
               alert("failed");
            });
            event.preventDefault();
        });

        $("#loginForm").on("submit", function (event) {
            var $this = $(this);
            var frmValues = $this.serialize();
            $.ajax({
                type: $this.attr('method'),
                url: $this.attr('action'),
                data: frmValues
            })
            .done(function (data) {
 
                MIM.login(data);
               


                })
            .fail(function () {
                alert("failed");
            });
            event.preventDefault();
        });

        $.validator.unobtrusive.parse("#loginForm");

        $.validator.unobtrusive.parse("#createCharaterForm");

      
        });
        
    });

 
