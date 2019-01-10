import React from 'react';
import { StyleSheet, Text, View, Image, AsyncStorage, Alert, TouchableHighlight, FlatList, TouchableWithoutFeedback, Button } from 'react-native';
import {PlayersData} from '../data/Players.js';
import { PlayerListItem } from '../sections/PlayerListItem.js';

export class MatchList extends React.Component {
    static navigationOptions = {
      title: 'Current Matches',
    };

    constructor(props) {
        super(props);
        this.state = {
            isInvited: false,
            loggedUser: false,
            matchList: []
        };
    }

    componentDidMount(){
        return fetch('http://35.228.60.109/api/match')
          .then((response) => response.json())
          .then((responseJson) => {

            console.log(responseJson);
            this.setState({
              matchList: responseJson,
            }, function(){
    
            });
    
          })
          .catch((error) =>{
            console.error(error);
          });
    }

    invitePlayers = () =>{
        this.setState({
            isInvited: true,
            playersList: Array.from(PlayersData.players)
        });
    }

    newMatch = () => {
        this.props.navigation.navigate('NewMatchRT');
    }

    render () {
        const { navigate } = this.props.navigation;

        return (
            <View style={styles.container}>
                
                <FlatList 
                            data={this.state.matchList}
                            renderItem={({item}) =>
                            <MatchItem
                                navigate={navigate}
                                id={item.id}
                                name={item.name} />
                            }    
                        />

                <Button title="New Match" onPress={this.newMatch} underlayColor='#31e981'  />
            </View>
        );
    }
}

export class MatchItem extends React.Component {
    onPress = () => {
        //Alert.alert(this.props.name);
        this.props.navigate('MatchInfoRT', {matchId: this.props.id} );
    }

    render(){
        return(
            <TouchableWithoutFeedback onPress={this.onPress}>
                <View style={{paddingTop:20,alignItems:'center'}}>
                    <Text>
                        {this.props.name}
                    </Text>
                </View>
            </TouchableWithoutFeedback>
        );
    }
}

export class PlayerList extends React.Component {
    onPress = () => {
        this.props.navigate('PlayerPickRT', {players: this.props.playersList} );
    }

    render(){
        return(
            <View>
                <TouchableHighlight onPress={this.onPress} underlayColor='#31e981'>
                    <Text style={styles.buttons}>Pick Players</Text>
                </TouchableHighlight>
                <FlatList style={{flex:1, margin: 10, justifyContent: 'space-between',width:'100%'}}                            
                            data={this.props.playersList}
                            renderItem={({item})=>
                                <Text style={{width:'100%'}}>{item.userName}</Text>
                            }
                        />
            </View>
        );
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems:'center',
        paddingBottom: '10%',
        paddingTop: '10%',
    },
    heading: {
        fontSize: 16,
        margin:10
    },
    inputs: {
        flex:1,
        width: '80%',
        padding: 10
    },
    buttons:{
        marginTop:15,
        fontSize:16
    },
    labels: {
        paddingBottom: 10
    }
});