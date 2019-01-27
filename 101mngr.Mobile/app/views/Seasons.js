import React from 'react';
import { View, TouchableWithoutFeedback, Text, FlatList, StyleSheet } from 'react-native'

export class Seasons extends React.Component {
    static navigationOptions = {
        title: 'Seasons'
    }

    constructor (props) {
        super(props);
        this.state = {
            leagueId: '',
            seasons: []
        };
    }

    componentDidMount = async () => {
        try {
            let leagueId = this.props.navigation.getParam('leagueId');
            this.setState({
                leagueId: leagueId,
              }, function(){});
            let response = await fetch(`http://35.228.60.109/api/leagues/${leagueId}/seasons`);
            let responseJson = await response.json();
            this.setState({
                seasons: responseJson,
            }, function(){
    
            });
        } catch (error) {
            console.error(error);
        }
    }

    render () {
        const { navigate } = this.props.navigation;

        return (
            <View style={styles.container}>
                
                <FlatList 
                            data={this.state.seasons}
                            keyExtractor={(item, index) => item.id}
                            renderItem={({item}) =>
                            <SeasonItem
                                navigate={navigate}
                                id={item.id}
                                leagueId={this.state.leagueId}
                                name={item.name} />
                            }    
                        />
            </View>
        );
    }
}

export class SeasonItem extends React.Component {
    onPress = () => {
        this.props.navigate('Teams', {seasonId: this.props.id, leagueId: this.props.leagueId} );
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